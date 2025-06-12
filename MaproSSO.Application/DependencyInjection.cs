using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using System.Reflection;
using MaproSSO.Application.Common.Behaviours;

namespace MaproSSO.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // AutoMapper
            services.AddAutoMapper(assembly);

            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantFilterBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            });

            // Application Services
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}