using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using VCard.Common.Application.EventBus;

namespace VCard.Common.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddEventBusWithTransport(
        this IServiceCollection services,
        Action<IRegistrationConfigurator, string>[] serviceConfigureConsumers,
        string serviceName,
        RabbitMqConfiguration rabbitMqConfiguration
    )
    {
        services.AddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(configure =>
        {
            var normalizedServiceName = serviceName
                .Replace(" ", "-")
                .Replace(".", "-")
                .ToLowerInvariant();

            foreach (var configureConsumer in serviceConfigureConsumers)
                configureConsumer(configure, normalizedServiceName);

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                {
                    h.Username(rabbitMqConfiguration.Username);
                    h.Password(rabbitMqConfiguration.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection RegisterIntegrationEventsHandlers(
        this IServiceCollection services,
        Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
            .ToArray();

        foreach (var handlerType in handlerTypes)
        {
            // Find the IIntegrationEventHandler<T> interface that this type implements
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

            // Register the concrete type against the interface
            services.AddTransient(handlerInterface, handlerType);

            Console.WriteLine($"Registered handler: {handlerType.Name} as {handlerInterface.Name}");
        }

        return services;
    }
}