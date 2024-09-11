using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace VCard.Users.Tests.Integration.Setup;

internal static class EventBusTestsExtensions
{
    public static WebApplicationFactory<T> WithTestEventBus<T>(
        this WebApplicationFactory<T> webApplicationFactory,
        params Type[] consumerTypes)
        where T : class
    {
        return webApplicationFactory.WithWebHostBuilder(webHostBuilder =>
            webHostBuilder.ConfigureTestServices(services =>
            {
                services.AddMassTransitTestHarness(configurator =>
                {
                    foreach (var consumerType in consumerTypes) configurator.AddConsumer(consumerType);
                });
            }));
    }
}