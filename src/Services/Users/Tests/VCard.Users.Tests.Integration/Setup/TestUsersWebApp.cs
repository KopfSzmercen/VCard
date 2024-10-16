using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Steeltoe.Common.Http.Discovery;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Tests.Integration.Setup;

public class UserIntegrationTestsBase : IAsyncLifetime
{
    private readonly TestDatabaseContainer _testDatabaseContainer = new();
    protected TestUsersWebApp App = null!;

    public async Task InitializeAsync()
    {
        await _testDatabaseContainer.InitializeAsync();

        App = new TestUsersWebApp(_testDatabaseContainer.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
        await _testDatabaseContainer.DisposeAsync();
    }

    protected class TestUsersWebApp(string databaseConnectionString) : WebApplicationFactory<Api.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((ctx, configurationBuilder) =>
            {
                var configurationPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "appsettings.IntegrationTests.json");

                configurationBuilder.AddJsonFile(configurationPath);
            });


            builder.ConfigureTestServices(services =>
            {
                services.Remove(
                    services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))!
                );
                services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(databaseConnectionString); });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(cfg =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.IntegrationTests.json")
                    .Build();

                cfg.AddConfiguration(configuration);
            });

            return base.CreateHost(builder);
        }
    }
}