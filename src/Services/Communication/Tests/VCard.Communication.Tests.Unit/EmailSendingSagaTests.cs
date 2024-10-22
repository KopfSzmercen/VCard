using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using VCard.Cards.IntegrationEvents;
using VCard.Communication.Api.Saga;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Tests.Unit;

public class EmailSendingSagaTests
{
    [Fact]
    public async Task GivenAllEventsAreConsumed_WhenEmailSent_ThenShouldTransitionToFinishedSendingEmailState()
    {
        //Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddSagaStateMachine<EmailSendingSaga, EmailSendingSagaData>(); })
            .BuildServiceProvider();

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var emailId = Guid.NewGuid();
        var senderId = Guid.NewGuid();

        //Act
        var emailSentEvent = new EmailSent(emailId, senderId);

        await harness.Bus.Publish(emailSentEvent);

        (await harness.Consumed.Any<EmailSent>()).Should().Be(true);

        var sagaHarness = harness.GetSagaStateMachineHarness<EmailSendingSaga, EmailSendingSagaData>();

        (await sagaHarness.Consumed.Any<EmailSent>()).Should().Be(true);

        (await sagaHarness.Created.Any(x => x.CorrelationId == emailId)).Should().Be(true);

        var moneyWithdrawnEvent = new MoneyWithdrawnIntegrationEvent(
            senderId,
            senderId,
            100,
            emailId
        );

        await harness.Bus.Publish(moneyWithdrawnEvent);

        (await sagaHarness.Consumed.Any<MoneyWithdrawnIntegrationEvent>()).Should().Be(true);

        var moneyWithdrawalConfirmationEmailSentEvent = new WithdrawMoneyConfirmationEmailSent(
            emailId,
            senderId
        );

        await harness.Bus.Publish(moneyWithdrawalConfirmationEmailSentEvent);

        (await sagaHarness.Consumed.Any<WithdrawMoneyConfirmationEmailSent>()).Should().Be(true);

        var sagaInstance =
            sagaHarness.Created.ContainsInState(emailId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);

        sagaInstance.CurrentState.Should().Be(sagaHarness.StateMachine.Final.Name);
    }
}