namespace VCard.Cards.Api.Cards.MoneyWithdrawal;

internal sealed record WithdrawMoney(
    Guid CardId,
    Money Money,
    DateTimeOffset WithdrawnAt
)
{
    public static MoneyWithdrawn Handle(WithdrawMoney command, Card card)
    {
        if (card.Money.Amount < command.Money.Amount) throw new InvalidOperationException("Insufficient funds.");

        return new MoneyWithdrawn(
            command.CardId,
            command.Money,
            command.WithdrawnAt
        );
    }
}

internal sealed record MoneyWithdrawn(
    Guid CardId,
    Money Money,
    DateTimeOffset WithdrawnAt
);