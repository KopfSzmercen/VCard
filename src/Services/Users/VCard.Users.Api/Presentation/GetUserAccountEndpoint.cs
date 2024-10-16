using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal sealed class GetUserAccountEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("{userId:guid}/account", Handle)
            .WithSummary("Get user account");
    }

    private static async Task<Results<
        Ok<Response>,
        NotFound<string>
    >> Handle(
        [FromServices] AppDbContext dbContext,
        [FromRoute] Guid userId
    )
    {
        var userAccount = await dbContext
            .UserAccount
            .Where(x => x.UserId == userId)
            .Select(x => new Response(
                x.Username,
                x.Address
            ))
            .FirstOrDefaultAsync();

        if (userAccount is null)
            return TypedResults.NotFound("User account not found");

        return TypedResults.Ok(userAccount);
    }

    private sealed record Response(string Username, string Address);
}