using System.Text.Json;
using VCard.Communication.Api.Presentation;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace VCard.Communication.Tests.Integration.Setup;

public class TestUsersService : IAsyncLifetime
{
    private WireMockServer _wireMockServer = null!;
    public string BaseAddress = null!;

    public Task InitializeAsync()
    {
        _wireMockServer = WireMockServer.Start();
        BaseAddress = _wireMockServer.Urls[0];
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _wireMockServer.Stop();
        return Task.CompletedTask;
    }

    public void SetupGetAccount200(Guid userId)
    {
        var expectedResponse = new SendEmailEndpoint.Client.Response(
            "test",
            "address"
        );

        _wireMockServer
            .Given(
                Request.Create()
                    .WithPath($"/users/{userId}/account")
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(expectedResponse))
            );
    }
}