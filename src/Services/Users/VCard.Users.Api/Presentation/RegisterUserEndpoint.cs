using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Auth;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("", Handle)
            .WithRequestValidation<Request>()
            .WithSummary("Register");
    }

    private static async Task<
        Results
        <BadRequest<ApiErrorResponse>, NoContent>
    > Handle(
        [FromServices] UserManager<User> userManager,
        [FromServices] AppDbContext dbContext,
        [FromBody] Request request
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await TryRegisterUser(request, userManager);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task<Results<BadRequest<ApiErrorResponse>, NoContent>> TryRegisterUser(Request request,
        UserManager<User> userManager)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Email
        };

        var createUserResult = await userManager.CreateAsync(user, request.Password);

        if (!createUserResult.Succeeded)
        {
            var error = createUserResult.Errors
                .First();

            return TypedResults.BadRequest(new ApiErrorResponse(error.Code, error.Description));
        }

        await userManager.AddToRolesAsync(user, [UserRole.User]);
        await userManager.AddClaimsAsync(user, [new Claim("id", user.Id.ToString())]);

        return TypedResults.NoContent();
    }

    public class Request
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string Email { get; init; }
        public string Password { get; init; }
    }

    internal sealed class RequestValidator : AbstractValidator<Request>
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