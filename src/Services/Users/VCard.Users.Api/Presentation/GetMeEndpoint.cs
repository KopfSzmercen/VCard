using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [FromServices] UserManager<User> userManager,
        [FromServices] IHttpContextAccessor httpContextAccessor
    )
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);

        return TypedResults.Ok(new Response(user!.Email!));
    }

    public sealed record Response(string Email);
}