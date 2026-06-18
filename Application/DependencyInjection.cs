using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Đăng ký toàn bộ UseCase (IRequestHandler) trong assembly Application
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Đăng ký toàn bộ Validator 
            services.AddValidatorsFromAssembly(assembly);

            // ValidationBehavior (pipeline) - chạy validator trước UseCase
            services.AddTransient(
                typeof(MediatR.IPipelineBehavior<,>),
                typeof(Application.Behaviors.ValidationBehavior<,>));

            return services;
        }
    }
}
