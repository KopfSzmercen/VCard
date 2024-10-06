using VCard.Cards.Api.Cards.Creating;
using VCard.Cards.Api.Cards.MoneyAdding;
using VCard.Cards.Api.Cards.MoneyWithdrawal;

namespace VCard.Cards.Api.Cards.GettingCard;

internal sealed record CardResponse
{
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Currency { get; set; } = null!;
    public decimal Amount { get; set; }
    public uint Version { get; set; }
}

internal sealed class CardResponseProjection
{
    public static CardResponse Handle(CardCreated @event)
    {
        return new CardResponse
        {
            CardId = @event.CardId,
            UserId = @event.UserId,
            CreatedAt = @event.CreatedAt,
            Currency = @event.Money.Currency,
            Amount = 0,
            Version = 0
        };
    }

    public static CardResponse Handle(MoneyAdded @event, CardResponse view)
    {
        view.Amount += @event.Money.Amount;
        view.Version += 1;

        return view;
    }

    public static CardResponse Handle(MoneyWithdrawn @event, CardResponse view)
    {
        view.Amount -= @event.Money.Amount;
        view.Version += 1;

        return view;
    }
}