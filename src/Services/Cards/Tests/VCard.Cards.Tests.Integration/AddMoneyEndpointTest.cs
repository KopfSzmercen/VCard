using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Presentation;
using VCard.Cards.Tests.Integration.Setup;
using VCard.Common.Auth.TokensManager;
using VCard.Users.IntegrationEvents;

namespace VCard.Cards.Tests.Integration;

public class AddMoneyEndpointTest(
    TestCardsWebApplication testCardsWebApplication
) : IClassFixture<TestCardsWebApplication>
{
    [Fact]
    public async Task GivenAddMoney_WhenAddingMoney_ThenReturnsOK_AndGetCard_ReturnsCard_WithIncreasedBalance()
    {
        // Arrange
        const int moneyToAdd = 100;
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

        // Act
        var addMoneyResponse =
            await client.PostAsJsonAsync("/cards/money",
                new AddMoneyEndpoint.Request(Money.AvailableCurrencies[0], moneyToAdd));

        // Assert
        addMoneyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await Task.Delay(3000);

        var getCardResponse = await client.GetAsync("/cards");

        getCardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getCardResponse.Content.ReadFromJsonAsync<GetCardEndpoint.Response>();

        content.Should().NotBeNull();
        content!.Amount.Should().Be(moneyToAdd);
    }
}