using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.Creating;
using VCard.Cards.Api.Cards.MoneyAdding;
using VCard.Cards.Api.Cards.MoneyWithdrawal;

namespace VCard.Cards.Tests.Unit;

public class CardTests
{
    [Fact]
    public void GivenCardCreatedEvent_CardShouldBeCreatedInInitialState()
    {
        // Arrange
        List<object> cardEvents =
            [new CardCreated(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, new Money(0, "USD"))];

        //Act
        var card = cardEvents.Aggregate(default(Card), Card.When);

        //Assert
        card.Should().NotBeNull();
        card.Money.Amount.Should().Be(0);
    }

    [Fact]
    public void GivenCardEventsStream_CardShouldBeUpdated()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        List<object> cardEvents = new()
        {
            new CardCreated(cardId, userId, DateTimeOffset.Now, new Money(0, "USD")),
            new MoneyAdded(cardId, new Money(100, "USD"), DateTimeOffset.Now),
            new MoneyWithdrawn(cardId, new Money(50, "USD"), DateTimeOffset.Now)
        };

        //Act
        var card = cardEvents.Aggregate(default(Card), Card.When);

        //Assert
        card.Should().NotBeNull();
        card.Money.Amount.Should().Be(50);
    }
}