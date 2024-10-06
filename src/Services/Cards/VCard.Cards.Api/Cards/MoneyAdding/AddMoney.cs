namespace VCard.Cards.Api.Cards.MoneyAdding;

internal sealed record MoneyAdded(
    Guid CardId,
    Money Money,
    DateTimeOffset AddedAt
);

internal sealed record AddMoney(
    Guid CardId,
    Money Money,
    DateTimeOffset AddedAt)
{
    public static MoneyAdded Handle(AddMoney command)
    {
        return new MoneyAdded(
            command.CardId,
            command.Money,
            command.AddedAt
        );
    }
}