namespace VCard.Cards.Api.Cards.Creating;

internal sealed record CreateCard(
    Guid UserId,
    DateTimeOffset CreatedAt,
    Money Money
)
{
    public static CardCreated Handle(CreateCard command)
    {
        return new CardCreated(
            command.UserId,
            command.UserId,
            command.CreatedAt,
            command.Money
        );
    }
}

internal sealed record CardCreated(
    Guid CardId,
    Guid UserId,
    DateTimeOffset CreatedAt,
    Money Money,
    uint Version = 0
);