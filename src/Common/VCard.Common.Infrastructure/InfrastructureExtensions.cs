using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VCard.Common.Application.EventBus;
using VCard.Common.Infrastructure.Outbox;

namespace VCard.Common.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddEventBusWithTransport<TDbContext>(
        this IServiceCollection services,
        Action<IRegistrationConfigurator, string>[] serviceConfigureConsumers,
        string serviceName,
        RabbitMqConfiguration rabbitMqConfiguration,
        bool useOutbox = false
    ) where TDbContext : DbContext
    {
        services.AddScoped<IEventBus, EventBus.EventBus>();

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

                cfg.ReceiveEndpoint();
            });

            //Check if db context is not the default one
            if (useOutbox && typeof(TDbContext) != typeof(DbContext)) configure.ConfigureOutbox<TDbContext>();
        });

        return services;
    }

    public static IServiceCollection AddEventBusWithTransport(
        this IServiceCollection services,
        Action<IRegistrationConfigurator, string>[] serviceConfigureConsumers,
        string serviceName,
        RabbitMqConfiguration rabbitMqConfiguration
    )
    {
        return AddEventBusWithTransport<DbContext>(
            services,
            serviceConfigureConsumers,
            serviceName,
            rabbitMqConfiguration
        );
    }

    public record SagaDefinition(Type SagaType, Type SagaDataType);

    public static IServiceCollection AddEventBusWithTransport(
        this IServiceCollection services,
        Action<IRegistrationConfigurator, string>[] serviceConfigureConsumers,
        string serviceName,
        RabbitMqConfiguration rabbitMqConfiguration,
        Action<IRegistrationConfigurator, string>[] configureSagas
    )
    {
        services.AddScoped<IEventBus, EventBus.EventBus>();

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

                cfg.ReceiveEndpoint();
            });

            configure.AddInMemoryInboxOutbox();

            foreach (var saga in configureSagas)
                saga(configure, normalizedServiceName);
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