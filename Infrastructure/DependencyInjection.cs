using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DB-first: connection string trỏ vào SQL Server đã có sẵn schema
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository - generic BaseRepository dùng chung
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();

            // UnitOfWork - chỉ quản lý transaction
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
