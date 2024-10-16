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

public class WithdrawMoneyEndpointTests(
    TestCardsWebApplication testCardsWebApplication
) : IClassFixture<TestCardsWebApplication>

{
    [Fact]
    public async Task GivenWithdrawMoney_WheWithdrawingMoney_ThenReturnsOK_AndGetCard_ReturnsCardWith_DecreasedBalance()
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

        await client.PostAsJsonAsync("/cards/money",
            new AddMoneyEndpoint.Request(Money.AvailableCurrencies[0], moneyToAdd));

        // Act
        var withdrawMoneyResponse =
            await client.PutAsJsonAsync("/cards/money/withdraw",
                new WithdrawMoneyEndpoint.Request(Money.AvailableCurrencies[0], moneyToAdd));

        //Assert
        withdrawMoneyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await Task.Delay(3000);

        var getCardResponse = await client.GetAsync("/cards");
        var getCardResponseContent = await getCardResponse.Content.ReadFromJsonAsync<GetCardEndpoint.Response>();

        getCardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getCardResponseContent.Should().NotBeNull();
        getCardResponseContent!.Amount.Should().Be(0);
    }
}