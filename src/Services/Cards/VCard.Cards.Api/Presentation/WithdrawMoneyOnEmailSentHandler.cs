using MassTransit;
using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.MoneyWithdrawal;
using VCard.Cards.Api.Cards.Persistence;
using VCard.Cards.IntegrationEvents;
using VCard.Communication.IntegrationEvents;

namespace VCard.Cards.Api.Presentation;

internal sealed class WithdrawMoneyOnEmailSentHandler(
    IEventStoreDbRepository<Card> repository
) : IConsumer<EmailSent>
{
    private const int MoneyToWithdraw = 5;

    public async Task Consume(ConsumeContext<EmailSent> context)
    {
        var card = await repository.FindAsync(
            Card.When,
            context.Message.SenderId.ToString(),
            CancellationToken.None
        );

        if (card is null) throw new Exception($"Card {context.Message.SenderId} not found");

        var moneyWithdrawn = WithdrawMoney.Handle(
            new WithdrawMoney(
                card.Id,
                new Money(MoneyToWithdraw, "USD"),
                DateTimeOffset.UtcNow
            ),
            card
        );

        await repository.AppendAsync(
            card.Id.ToString(),
            moneyWithdrawn,
            card.Version,
            CancellationToken.None
        );

        await context.Publish(
            new MoneyWithdrawnIntegrationEvent(
                card.UserId,
                card.UserId,
                MoneyToWithdraw,
                context.Message.EmailId
            )
        );
    }
}