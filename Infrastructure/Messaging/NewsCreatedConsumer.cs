using Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Application.Interfaces;
using Application.DTOs;
using Application.Events;
using Domain.Interfaces;

namespace Infrastructure.Messaging
{
    public class NewsCreatedConsumer : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<NewsCreatedConsumer> _logger;

        private IConnection? _connection;
        private IModel? _channel;

        public NewsCreatedConsumer(
            IServiceProvider services,
            IOptions<RabbitMqOptions> options,
            ILogger<NewsCreatedConsumer> logger)
        {
            _services = services;
            _options = options.Value;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
               
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true);

            
            _channel.QueueDeclare("news.sync.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("news.sync.queue", _options.ExchangeName, "news.*");
            _channel.BasicQos(0, 1, false); 

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceivedAsync;

            _channel.BasicConsume("news.sync.queue", autoAck: false, consumer);
            _logger.LogInformation("NewsCreatedConsumer đang lắng nghe queue: news.sync.queue");

            return Task.CompletedTask;
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var routingKey = ea.RoutingKey;
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Nhận event [{RoutingKey}] từ RabbitMQ", routingKey);

            try
            {
                using var scope = _services.CreateScope();
                var readRepo = scope.ServiceProvider.GetRequiredService<INewsReadRepository>();
                var menuRepo = scope.ServiceProvider.GetRequiredService<IMenuRepository>();

                
                if (routingKey == "news.created" || routingKey == "news.updated")
                {
                    var eventDoc = JsonDocument.Parse(body).RootElement;
                    var newsId = eventDoc.GetProperty("NewsId").GetGuid();
                    var title = eventDoc.GetProperty("Title").GetString()!;
                    var content = eventDoc.GetProperty("Content").GetString()!;
                    var summary = eventDoc.GetProperty("Summary").GetString();
                    var isPublished = eventDoc.GetProperty("IsPublished").GetBoolean();

                    
                    var menuIds = eventDoc.GetProperty("MenuIds").EnumerateArray().Select(x => x.GetGuid()).ToList();
                    var menus = new List<MenuDto>();
                    foreach (var menuId in menuIds)
                    {
                        var menu = await menuRepo.GetByIdAsync(menuId);
                        if (menu != null)
                            menus.Add(new MenuDto(menu.Id, menu.Name, menu.Slug, menu.DisplayOrder));
                    }

                    var newsDto = new NewsDto(newsId, title, content, summary, isPublished, DateTime.UtcNow, menus);

                    await readRepo.UpsertAsync(newsDto);
                    _logger.LogInformation("Ghi dữ liệu News {NewsId} vào MongoDB thành công", newsId);
                }

                
                else if (routingKey == "news.deleted")
                {
                    var eventDoc = JsonDocument.Parse(body).RootElement;
                    var newsId = eventDoc.GetProperty("NewsId").GetGuid();

                    
                    await readRepo.DeleteAsync(newsId);
                    _logger.LogInformation("Đã XÓA VĨNH VIỄN News {NewsId} trên MongoDB", newsId);
                }

                
                _channel!.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý message RabbitMQ [{RoutingKey}]", routingKey);
                
                _channel!.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
