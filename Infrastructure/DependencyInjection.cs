using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Interfaces;                 
using Infrastructure.Persistence;             
using Infrastructure.Persistence.Repositories;
using Infrastructure.Options;                  
using Infrastructure.Repositories;           
using Infrastructure.Messaging;             

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // SQL Server
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(config.GetConnectionString("SqlServer")));

            // Repositories va Unit of work
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // MongoDB Read
            services.Configure<MongoDbOptions>(config.GetSection("MongoDB"));
            services.AddScoped<INewsReadRepository, NewsMongoReadRepository>();

            // RabbitMQ Publisher
            services.Configure<RabbitMqOptions>(config.GetSection("RabbitMQ"));
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

            // Consumer chạy nền
            services.AddHostedService<NewsCreatedConsumer>();

            return services;
        }
    }
}