using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VCard.Common.Auth.TokensManager;
using VCard.Communication.Api.Presentation;
using VCard.Communication.Tests.Integration.Setup;

namespace VCard.Communication.Tests.Integration;

public class SendEmailEndpointTests(
    TestWebApp testWebApp
) : IClassFixture<TestWebApp>
{
    [Fact]
    public async Task GivenUserAccountExists_WhenSendEmailRequestIsReceived_ThenEmailIsSent()
    {
        // Arrange
        var client = testWebApp.CreateClient();
        var userId = Guid.NewGuid();
        client.Authenticate(userId, testWebApp.Services.GetRequiredService<ITokensManager>());

        var request = new SendEmailEndpoint.Request
        {
            Subject = "Test subject",
            Body = "Test body",
            To = "test@t.pl"
        };

        testWebApp.TestUsersService.SetupGetAccount200(userId);

        // Act
        var response = await client.PostAsJsonAsync("/v1/emails", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}