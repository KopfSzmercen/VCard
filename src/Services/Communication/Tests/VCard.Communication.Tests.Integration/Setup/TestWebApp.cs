using Consul;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using VCard.Common.Auth;
using VCard.Common.Auth.TokensManager;
using VCard.Communication.Api.ExternalServices;
using VCard.Communication.Api.Persistence;

namespace VCard.Communication.Tests.Integration.Setup;

[UsedImplicitly]
public sealed class TestWebApp : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestDatabaseContainer _testDatabaseContainer = new();
    private readonly TestConsulContainer _testConsulContainer = new();
    public readonly TestUsersService TestUsersService = new();

    public async Task InitializeAsync()
    {
        await _testDatabaseContainer.InitializeAsync();
        await _testConsulContainer.InitializeAsync();
        await TestUsersService.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _testDatabaseContainer.DisposeAsync();
        await _testConsulContainer.DisposeAsync();
        await TestUsersService.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.IntegrationTests.json")
            .Build();

        builder.ConfigureTestServices(services =>
        {
            services.Remove(
                services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))!
            );

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_testDatabaseContainer.ConnectionString);
            });

            services.AddMassTransitTestHarness(configure =>
            {
                configure.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
            });

            services.Configure<JwtTokensOptions>(configuration.GetSection(JwtTokensOptions.SectionName));

            services.AddSingleton<ITokensManager, TokensManager>();

            services.RemoveAll(typeof(ConsulClient));

            var consulClient =
                new ConsulClient(c =>
                    c.Address = new Uri(_testConsulContainer.ConsulContainer.GetBaseAddress())
                );

            consulClient.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = ExternalServicesRegistry.UsersApi,
                Name = ExternalServicesRegistry.UsersApi,
                Address = new Uri(TestUsersService.BaseAddress).Host,
                Port = new Uri(TestUsersService.BaseAddress).Port
            });

            services.AddSingleton(consulClient);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(cfg =>
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.IntegrationTests.json")
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Consul:Port"] = new Uri(
                            _testConsulContainer.ConsulContainer.GetBaseAddress(), UriKind.Absolute
                        ).Port.ToString(),
                        ["Consul:Host"] = _testConsulContainer.ConsulContainer.Hostname
                    }!
                )
                .Build();

            cfg.AddConfiguration(configuration);
        });

        return base.CreateHost(builder);
    }
}