using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VCard.Common.Application.EventBus;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Auth;
using VCard.Users.Api.Persistence;
using VCard.Users.IntegrationEvents;

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
        <BadRequest<ApiErrorResponse>, Ok<Guid>>
    > Handle(
        [FromServices] UserManager<User> userManager,
        [FromServices] AppDbContext dbContext,
        [FromBody] Request request,
        [FromServices] IEventBus eventBus
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await TryRegisterUser(request, userManager);

            if (!result.IsSuccess)
            {
                await transaction.RollbackAsync();
                return TypedResults.BadRequest(result.Error!);
            }

            await eventBus.PublishAsync(new UserRegistered
            {
                UserEmail = request.Email,
                Id = Guid.NewGuid(),
                OccurredOn = DateTimeOffset.Now,
                UserId = result.UserId!.Value
            });

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return TypedResults.Ok(result.UserId!.Value);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task<RegisterUserResult> TryRegisterUser(Request request,
        UserManager<User> userManager)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            Id = Guid.NewGuid()
        };

        var createUserResult = await userManager.CreateAsync(user, request.Password);

        if (!createUserResult.Succeeded)
        {
            var error = createUserResult.Errors
                .First();

            return new RegisterUserResult(new ApiErrorResponse(error.Code, error.Description), null);
        }

        await userManager.AddToRolesAsync(user, [UserRole.User]);
        await userManager.AddClaimsAsync(user, [
            new Claim("UserId", user.Id.ToString()),
            new Claim("SendEmails", true.ToString())
        ]);

        return new RegisterUserResult(null, user.Id);
    }

    private sealed record RegisterUserResult(
        ApiErrorResponse? Error,
        Guid? UserId
    )
    {
        public bool IsSuccess => Error is null;
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