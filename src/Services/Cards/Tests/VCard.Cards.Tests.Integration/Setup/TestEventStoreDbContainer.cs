using Testcontainers.EventStoreDb;

namespace VCard.Cards.Tests.Integration.Setup;

public class TestEventStoreDbContainer : IAsyncLifetime
{
    private const string Username = "admin";

    private const string Password = "password";

    private const string Database = "vcard-test";

    private readonly EventStoreDbContainer _eventStoreDbContainer = new EventStoreDbBuilder()
        .WithImage("eventstore/eventstore:24.2.0-jammy")
        .WithHostname("vcard-eventstore")
        .Build();

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        await _eventStoreDbContainer.StartAsync();
        ConnectionString = _eventStoreDbContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _eventStoreDbContainer.StopAsync();
    }
}