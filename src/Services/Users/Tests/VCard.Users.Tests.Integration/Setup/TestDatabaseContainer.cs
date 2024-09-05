using Testcontainers.PostgreSql;

namespace VCard.Users.Tests.Integration.Setup;

internal sealed class TestDatabaseContainer : IAsyncLifetime
{
    private const string Username = "postgres";

    private const string Password = "password";

    private const string Database = "vcard-test";

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase(Database)
        .WithUsername(Username)
        .WithPassword(Password)
        .Build();

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        ConnectionString = _postgresContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
}