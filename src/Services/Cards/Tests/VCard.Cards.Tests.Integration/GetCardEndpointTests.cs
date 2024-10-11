using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using VCard.Cards.Api.Presentation;
using VCard.Cards.Tests.Integration.Setup;
using VCard.Common.Auth.TokensManager;
using VCard.Users.IntegrationEvents;

namespace VCard.Cards.Tests.Integration;

public class GetCardEndpointTests(
    TestCardsWebApplication testCardsWebApplication
) : IClassFixture<TestCardsWebApplication>
{
    [Fact]
    public async Task GivenUserRegisteredEvent_WhenGettingCards_ThenReturnsCard_WithZeroBalance()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var testHarness = testCardsWebApplication.Services.GetTestHarness();
        using var client = testCardsWebApplication.CreateClient();
        var tokensManager = testCardsWebApplication.Services.GetRequiredService<ITokensManager>();
        client.Authenticate(userId, tokensManager);

        await testHarness.Bus.Publish(new UserRegistered
        {
            Id = userId,
            UserEmail = "testemail@t.pl",
            OccurredOn = DateTimeOffset.Now,
            UserId = userId
        });

        await Task.Delay(3000);

        // Act
        var response = await client.GetAsync("/cards");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<GetCardEndpoint.Response>();
        content.Should().NotBeNull();
    }
}