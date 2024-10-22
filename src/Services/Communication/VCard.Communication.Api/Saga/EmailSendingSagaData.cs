using MassTransit;

namespace VCard.Communication.Api.Saga;

public class EmailSendingSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public Guid SenderId { get; set; }

    public bool EmailSent { get; set; }

    public bool MoneyWithdrawn { get; set; }

    public bool WithdrawMoneyConfirmationEmailSent { get; set; }
}