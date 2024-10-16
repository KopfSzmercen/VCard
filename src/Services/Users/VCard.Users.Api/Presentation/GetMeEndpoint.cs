using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VCard.Common.Application.RequestContext;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal class GetMeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("me", Handle)
            .RequireAuthorization()
            .WithSummary("Get me")
            .Produces<UnauthorizedResult>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, Ok<Response>>> Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] IRequestContext requestContext
    )
    {
        var user = await dbContext
            .Users
            .Where(x => x.Id == requestContext.Id)
            .Include(x => x.UserAccount)
            .Select(x => new Response(
                x.Email!,
                x.UserName!,
                x.UserAccount == null
                    ? null
                    : new AccountResponse(
                        x.UserAccount.Username,
                        x.UserAccount.Address
                    )
            ))
            .FirstOrDefaultAsync();

        return TypedResults.Ok(user!);
    }

    public sealed record Response(string Email, string Username, AccountResponse? Account = null);

    public sealed record AccountResponse(string Username, string Address);
}