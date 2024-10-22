namespace VCard.Cards.IntegrationEvents;

public sealed record MoneyWithdrawnIntegrationEvent(
    Guid CardId,
    Guid OwnerId,
    int Amount,
    Guid EmailId
);