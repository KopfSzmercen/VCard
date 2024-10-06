using VCard.Cards.Api.Cards.Creating;
using VCard.Cards.Api.Cards.MoneyAdding;
using VCard.Cards.Api.Cards.MoneyWithdrawal;

namespace VCard.Cards.Api.Cards;

internal sealed record Card(
    Guid Id,
    Guid UserId,
    Money Money,
    DateTimeOffset CreatedAt,
    uint Version = 0
)
{
    public static Card When(Card? entity, object @event)
    {
        if (@event is CardCreated e)
            return new Card(e.CardId, e.UserId, new Money(0, Money.AvailableCurrencies[0]), e.CreatedAt);

        if (entity == null)
            throw new ArgumentException("Entity cannot be null for non-creation events", nameof(entity));

        switch (@event)
        {
            case MoneyWithdrawn withdrawEvent:
            {
                return entity with
                {
                    Money = new Money(entity.Money.Amount - withdrawEvent.Money.Amount, entity.Money.Currency),
                    Version = entity.Version + 1
                };
            }
            case MoneyAdded addEvent:
            {
                return entity with
                {
                    Money = new Money(entity.Money.Amount + addEvent.Money.Amount, entity.Money.Currency),
                    Version = entity.Version + 1
                };
            }
        }

        throw new ArgumentException("Unsupported event type", nameof(@event));
    }
}