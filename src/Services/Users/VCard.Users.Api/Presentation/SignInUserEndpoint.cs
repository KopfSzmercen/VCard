using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal sealed class SignInUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("sign-in", Handle)
            .WithRequestValidation<Reuqest>()
            .WithSummary("Sign in user with cookie");
    }

    private static async Task<
        Results<
            BadRequest<InvalidCredentialsError>,
            BadRequest<TooManySignInAttemptsError>,
            Ok>
    > Handle(
        [FromServices] SignInManager<User> signInManager,
        [FromServices] UserManager<User> userManager,
        [FromBody] Reuqest request
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return TypedResults.BadRequest(new InvalidCredentialsError());

        var signInResult = await signInManager
            .PasswordSignInAsync(user, request.Password, true, false);

        if (signInResult.IsLockedOut)
            return TypedResults.BadRequest(new TooManySignInAttemptsError());

        if (!signInResult.Succeeded)
            return TypedResults.BadRequest(new InvalidCredentialsError());

        return TypedResults.Ok();
    }

    public sealed record Reuqest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string Email { get; init; }
        public string Password { get; init; }
    }

    private sealed record InvalidCredentialsError()
        : ApiErrorResponse("UserNotFound", "User not found.");

    private sealed record TooManySignInAttemptsError()
        : ApiErrorResponse("TooManySignInAttempts", "Too many sign-in attempts.");

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