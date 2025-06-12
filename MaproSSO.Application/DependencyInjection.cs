using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MaproSSO.Application.Common.Behaviours;
using MaproSSO.Application.Common.Mappings;

namespace MaproSSO.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<TenantMappingProfile>();
            cfg.AddProfile<SubscriptionMappingProfile>();
            cfg.AddProfile<SecurityMappingProfile>();
            cfg.AddProfile<SSOMappingProfile>();
            cfg.AddProfile<InspectionMappingProfile>();
            cfg.AddProfile<AuditMappingProfile>();
            cfg.AddProfile<AccidentMappingProfile>();
            cfg.AddProfile<TrainingMappingProfile>();
        });

        // Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}