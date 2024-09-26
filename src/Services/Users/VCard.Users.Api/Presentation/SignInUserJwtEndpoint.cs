using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Auth.Tokens;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal sealed class SignInUserJwtEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("sign-in-jwt", Handle)
            .WithRequestValidation<RequestValidator>()
            .WithSummary("Sign in user and return JWT token");
    }

    private static async Task<Results<
        BadRequest<InvalidCredentialsError>,
        BadRequest<TooManySignInAttemptsError>,
        Ok<Response>
    >> Handle(
        [FromServices] SignInManager<User> signInManager,
        [FromServices] UserManager<User> userManager,
        [FromServices] ITokensManager tokensManager,
        [FromBody] Reuqest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return TypedResults.BadRequest(new InvalidCredentialsError());

        var signInResult = await signInManager
            .PasswordSignInAsync(user, request.Password, false, false);

        if (signInResult.IsLockedOut)
            return TypedResults.BadRequest(new TooManySignInAttemptsError());

        if (!signInResult.Succeeded)
            return TypedResults.BadRequest(new InvalidCredentialsError());

        var userRoles = await userManager.GetRolesAsync(user);

        var userClaims = await userManager.GetClaimsAsync(user);

        foreach (var role in userRoles) userClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = tokensManager.CreateToken(
            user.Id,
            userRoles.ToList(),
            //Temporary solution to add ManageCards claim
            [..userClaims.ToList(), new Claim("ManageCards", true.ToString())]
        );

        return TypedResults.Ok(new Response(token.AccessToken));
    }


    private sealed record InvalidCredentialsError()
        : ApiErrorResponse("UserNotFound", "User not found.");

    private sealed record TooManySignInAttemptsError()
        : ApiErrorResponse("TooManySignInAttempts", "Too many sign-in attempts.");

    public sealed record Response(string Token);

    public sealed record Reuqest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string Email { get; init; }
        public string Password { get; init; }
    }

    internal sealed class RequestValidator : AbstractValidator<Reuqest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}