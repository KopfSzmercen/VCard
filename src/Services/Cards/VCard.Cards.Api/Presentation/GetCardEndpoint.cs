using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.Persistence;
using VCard.Cards.Api.Persistence;
using VCard.Common.Application.RequestContext;
using VCard.Common.Presentation.Endpoints;

namespace VCard.Cards.Api.Presentation;

internal class GetCardEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", Handle)
            .RequireAuthorization()
            .WithSummary("Get card by id");
    }

    private static async Task<Results<NotFound<string>, Ok<Response>>> Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] IRequestContext requestContext
    )
    {
        var card = await dbContext.CardResponses
            .AsNoTracking()
            .Where(x => x.UserId == requestContext.Id)
            .Select(card => new Response(
                card.CardId,
                card.UserId,
                card.Currency,
                card.Amount,
                card.CreatedAt
            ))
            .FirstOrDefaultAsync();

        if (card is null)
            return TypedResults
                .NotFound("Card not found");

        return TypedResults.Ok(card);
    }

    private sealed record Response(
        Guid Id,
        Guid UserId,
        string Currency,
        decimal Amount,
        DateTimeOffset CreatedAt
    );
}