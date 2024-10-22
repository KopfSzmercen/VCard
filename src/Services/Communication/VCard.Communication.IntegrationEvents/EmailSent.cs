namespace VCard.Communication.IntegrationEvents;

public sealed record class EmailSent(Guid EmailId, Guid SenderId);