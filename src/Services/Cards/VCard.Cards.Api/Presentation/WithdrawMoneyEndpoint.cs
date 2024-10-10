using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.MoneyWithdrawal;
using VCard.Cards.Api.Cards.Persistence;
using VCard.Common.Application.RequestContext;
using VCard.Common.Presentation.Endpoints;

namespace VCard.Cards.Api.Presentation;

internal sealed class WithdrawMoneyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/money/withdraw", Handle)
            .WithSummary("Withdraw money from card")
            .WithRequestValidation<RequestValidator>()
            .RequireAuthorization();
    }

    private static async Task<Results<BadRequest<string>, Ok>> Handle(
        [FromServices] IEventStoreDbRepository<Card> repository,
        [FromBody] Request request,
        [FromServices] IRequestContext requestContext,
        CancellationToken cancellationToken
    )
    {
        var card = await repository.FindAsync(Card.When, requestContext.Id.ToString()!, CancellationToken.None);

        if (card is null)
            return TypedResults
                .BadRequest("Card not found");

        var moneyWithdrawn = WithdrawMoney.Handle(new WithdrawMoney(
                card.Id,
                new Money(request.Amount, request.Currency),
                DateTimeOffset.UtcNow
            ),
            card
        );

        await repository.AppendAsync(card.Id.ToString(), moneyWithdrawn, card.Version, cancellationToken);

        return TypedResults.Ok();
    }

    public sealed record Request(
        string Currency,
        int Amount
    );

    private sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Currency)
                .NotEmpty()
                .Must(BeAValidCurrency)
                .WithMessage("Invalid currency");

            RuleFor(x => x.Amount)
                .NotEmpty();
        }

        private static bool BeAValidCurrency(string currency)
        {
            return Money.AvailableCurrencies.Contains(currency);
        }
    }
}