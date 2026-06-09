using Application.Events;
using Application.Interfaces;
using MediatR;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.News.Commands.CreateNews
{
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<CreateNewsCommandHandler> _logger;

        public CreateNewsCommandHandler(
            IUnitOfWork uow,
            IEventPublisher eventPublisher,
            ILogger<CreateNewsCommandHandler> logger)
        {
            _uow = uow;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateNewsCommand request, CancellationToken ct)
        {
            //Tạo News entity
            var news = new Domain.Entities.News
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Summary = request.Summary,
                IsPublished = request.IsPublished,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Tạo quan hệ MenuNews 
            var menuNewsItems = request.MenuIds.Select(menuId => new MenuNews
            {
                MenuId = menuId,
                NewsId = news.Id,
                AssignedAt = DateTime.UtcNow,
            }).ToList();

            news.MenuNews = menuNewsItems;

            try
            {
                
                await _uow.News.AddAsync(news, ct);

               
                await _uow.CommitAsync(ct);

                _logger.LogInformation("News {NewsId} da duoc luu vao SQL Server", news.Id);

                
                var @event = new NewsCreatedEvent(
                    news.Id, news.Title, news.Content,
                    news.Summary, news.IsPublished, news.CreatedAt,
                    request.MenuIds);

                await _eventPublisher.PublishAsync("news.created", @event, ct);
                _logger.LogInformation("Event NewsCreated published cho News {NewsId}", news.Id);

                return news.Id;
            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex, "Loi khi tao news ,dang rollback transaction");
                await _uow.RollbackAsync();
                throw; 
            }
        }
    }
}
