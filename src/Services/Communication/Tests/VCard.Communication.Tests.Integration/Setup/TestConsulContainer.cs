using Testcontainers.Consul;

namespace VCard.Communication.Tests.Integration.Setup;

public class TestConsulContainer : IAsyncLifetime
{
    public readonly ConsulContainer ConsulContainer = new ConsulBuilder()
        .WithImage("hashicorp/consul:latest")
        .Build();

    public async Task InitializeAsync()
    {
        await ConsulContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await ConsulContainer.StopAsync();
    }
}