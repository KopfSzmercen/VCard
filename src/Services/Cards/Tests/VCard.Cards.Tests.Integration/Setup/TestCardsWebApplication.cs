using JetBrains.Annotations;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VCard.Cards.Api.EventStore;
using VCard.Cards.Api.Persistence;
using VCard.Common.Auth;
using VCard.Common.Auth.TokensManager;

namespace VCard.Cards.Tests.Integration.Setup;

[UsedImplicitly]
public class TestCardsWebApplication : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestPostgresDbContainer _postgresDbContainer = new();
    private readonly TestEventStoreDbContainer _eventStoreDbContainer = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var configurationPath = Path.Combine(Directory.GetCurrentDirectory(),
                "appsettings.IntegrationTests.json");

            configurationBuilder.AddJsonFile(configurationPath);

            //add event store configuration
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "EventStore:ConnectionString", _eventStoreDbContainer.ConnectionString },
                {
                    "Jwt:SigningKey",
                    "9c6e116755e3d00729e8746e064b28906208001b75c7cefa19527eb4fa05a29e1cabbb57ea681d75c83da27fcb93c94763730ea47cc8fa4985930bb886185984ab93a8ea762402a06cd4cc0ff303306cff6948c5e53501d177661395d2c54aff943d79576b06fdeade1b95aaed093fd5203b35bb5f36f2d"
                },
                { "Jwt:Issuer", "Vcard" },
                { "Jwt:Audience", "VcardUsers" }
            }!);
        });

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.IntegrationTests.json")
            .Build();

        builder.ConfigureTestServices(services =>
        {
            services.Remove(services.SingleOrDefault(x => x.ServiceType == typeof(EventStoreOptions))!);

            services.AddSingleton(new EventStoreOptions
            {
                ConnectionString = _eventStoreDbContainer.ConnectionString
            });

            services.Remove(
                services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))!
            );

            services.AddDbContext<AppDbContext>(
                options => { options.UseNpgsql(_postgresDbContainer.ConnectionString); });

            services.AddMassTransitTestHarness(configure =>
            {
                configure.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
            });

            services.Configure<JwtTokensOptions>(configuration.GetSection(JwtTokensOptions.SectionName));

            services.AddSingleton<ITokensManager, TokensManager>();
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresDbContainer.InitializeAsync();
        await _eventStoreDbContainer.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgresDbContainer.DisposeAsync();
        await _eventStoreDbContainer.DisposeAsync();
    }
}