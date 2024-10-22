namespace VCard.Communication.IntegrationEvents;

public sealed record WithdrawMoneyConfirmationEmailSent(
    Guid EmailId,
    Guid UserId
);