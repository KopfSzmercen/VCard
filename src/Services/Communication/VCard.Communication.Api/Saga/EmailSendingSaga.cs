using MassTransit;
using VCard.Cards.IntegrationEvents;
using VCard.Communication.Api.Persistence;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Api.Saga;

internal sealed class EmailSendingSaga : MassTransitStateMachine<EmailSendingSagaData>
{
    public State FinishedSendingEmail { get; set; } = null!;

    public State FinishedWithdrawingMoney { get; set; } = null!;

    public State FinishedSendingConfirmationEmail { get; set; } = null!;

    public State Compensating { get; set; } = null!;

    public State Compensated { get; set; } = null!;

    public Event<EmailSent> EmailSent { get; set; } = null!;

    public Event<MoneyWithdrawnIntegrationEvent> MoneyWithdrawn { get; set; } = null!;

    public Event<WithdrawMoneyConfirmationEmailSent> WithdrawMoneyConfirmationEmailSent { get; set; } = null!;

    public Event<Fault<EmailSent>> EmailSentConsumersFault { get; set; } = null!;

    public Event<RevertedSendingEmail> RevertedSendingEmail { get; set; } = null!;

    public EmailSendingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(
            () => EmailSent,
            e => e.CorrelateById(m => m.Message.EmailId));

        Event(
            () => MoneyWithdrawn,
            e => e.CorrelateById(m => m.Message.EmailId));

        Event(
            () => WithdrawMoneyConfirmationEmailSent,
            e => e.CorrelateById(m => m.Message.EmailId));

        Event(
            () => EmailSentConsumersFault,
            e => e.CorrelateById(m => m.Message.Message.EmailId)
                .SelectId(ctx => ctx.Message.Message.EmailId)
        );

        Event(
            () => RevertedSendingEmail,
            e => e.CorrelateById(m => m.Message.EmailId)
        );

        DuringAny(
            When(EmailSentConsumersFault)
                .TransitionTo(Compensating)
                .Publish(
                    ctx => new EmailSentConsumersFailed(ctx.Message.Message.EmailId)
                )
        );

        During(
            Compensating,
            When(RevertedSendingEmail)
                .Then(ctx => { ctx.Saga.EmailSent = false; })
                .TransitionTo(Compensated)
        );

        Initially(
            When(EmailSent)
                .Then(ctx =>
                {
                    ctx.Saga.SenderId = ctx.Message.SenderId;
                    ctx.Saga.EmailSent = true;
                })
                .TransitionTo(FinishedSendingEmail)
        );

        Initially(
            When(MoneyWithdrawn)
                .Then(ctx =>
                {
                    ctx.Saga.SenderId = ctx.Message.OwnerId;
                    ctx.Saga.MoneyWithdrawn = true;
                })
                .TransitionTo(FinishedWithdrawingMoney)
        );

        Initially(
            When(WithdrawMoneyConfirmationEmailSent)
                .Then(ctx =>
                {
                    ctx.Saga.SenderId = ctx.Message.UserId;
                    ctx.Saga.WithdrawMoneyConfirmationEmailSent = true;
                })
                .TransitionTo(FinishedSendingConfirmationEmail)
        );


        During(
            FinishedSendingEmail,
            When(MoneyWithdrawn)
                .Then(ctx => { ctx.Saga.MoneyWithdrawn = true; })
                .TransitionTo(FinishedWithdrawingMoney),
            When(WithdrawMoneyConfirmationEmailSent).Then(
                    ctx => { ctx.Saga.WithdrawMoneyConfirmationEmailSent = true; }
                )
                .TransitionTo(FinishedSendingConfirmationEmail)
        );

        During(
            FinishedWithdrawingMoney,
            When(EmailSent)
                .Then(ctx => { ctx.Saga.EmailSent = true; })
                .TransitionTo(FinishedSendingEmail),
            When(WithdrawMoneyConfirmationEmailSent).Then(
                    ctx => { ctx.Saga.WithdrawMoneyConfirmationEmailSent = true; }
                )
                .TransitionTo(FinishedSendingConfirmationEmail)
        );

        During(
            FinishedSendingConfirmationEmail,
            When(EmailSent)
                .Then(ctx => { ctx.Saga.EmailSent = true; })
                .TransitionTo(FinishedSendingEmail),
            When(MoneyWithdrawn)
                .Then(ctx => { ctx.Saga.MoneyWithdrawn = true; })
                .TransitionTo(FinishedWithdrawingMoney)
        );

        DuringAny(
            When(EmailSent)
                .Then(ctx => { ctx.Saga.EmailSent = true; })
                .If(
                    ctx => ctx.Saga is
                        { MoneyWithdrawn: true, WithdrawMoneyConfirmationEmailSent: true, EmailSent: true },
                    activity => activity.Finalize()
                ),
            When(MoneyWithdrawn)
                .Then(ctx => { ctx.Saga.MoneyWithdrawn = true; })
                .If(
                    ctx => ctx.Saga is
                        { MoneyWithdrawn: true, WithdrawMoneyConfirmationEmailSent: true, EmailSent: true },
                    activity => activity.Finalize()
                ),
            When(WithdrawMoneyConfirmationEmailSent)
                .Then(ctx => { ctx.Saga.WithdrawMoneyConfirmationEmailSent = true; })
                .If(
                    ctx => ctx.Saga is
                        { MoneyWithdrawn: true, WithdrawMoneyConfirmationEmailSent: true, EmailSent: true },
                    activity => activity.Finalize()
                )
        );
    }

    public static Action<IRegistrationConfigurator, string> Register()
    {
        return (registrationConfigurator, _) =>
        {
            registrationConfigurator.AddSagaStateMachine<
                EmailSendingSaga,
                EmailSendingSagaData
            >(
                cfg =>
                {
                    const int concurrencyLimit = 20;
                    var partition = cfg.CreatePartitioner(concurrencyLimit);

                    cfg.UseMessageRetry(r => r.Interval(5, 1000));

                    cfg.Message<EmailSent>(m =>
                        m.UsePartitioner(partition, message => message.Message.SenderId)
                    );

                    cfg.Message<MoneyWithdrawnIntegrationEvent>(m =>
                        m.UsePartitioner(partition, message => message.Message.OwnerId)
                    );

                    cfg.Message<WithdrawMoneyConfirmationEmailSent>(m =>
                        m.UsePartitioner(partition, message => message.Message.UserId)
                    );
                }
            ).EntityFrameworkRepository(
                r =>
                {
                    r.ExistingDbContext<AppDbContext>();
                    r.UsePostgres();
                }
            );
        };
    }
}