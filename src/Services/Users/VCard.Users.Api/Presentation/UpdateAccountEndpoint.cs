using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VCard.Common.Application.RequestContext;
using VCard.Common.Presentation.Endpoints;
using VCard.Users.Api.Persistence;

namespace VCard.Users.Api.Presentation;

internal sealed class UpdateAccountEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("account", Handle)
            .RequireAuthorization()
            .WithSummary("Update account")
            .WithRequestValidation<UpdateAccountRequestValidator>();
    }

    private static async Task<
        Results<NoContent, BadRequest<string>>
    > Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] IRequestContext requestContext,
        [FromBody] Request request,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users
            .Include(x => x.UserAccount)
            .Where(x => x.Id == requestContext.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return TypedResults.BadRequest("User not found");

        if (user.UserAccount is null)
        {
            var newUserAccount = new UserAccount
            {
                Username = request.Username,
                Address = request.Address,
                UserId = user.Id,
                User = user
            };

            await dbContext.UserAccount.AddAsync(newUserAccount, cancellationToken);
        }

        else
        {
            user.UserAccount.Username = request.Username;
            user.UserAccount.Address = request.Address;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    public sealed record Request
    {
        public required string Username { get; init; }
        public required string Address { get; init; }
    }

    private class UpdateAccountRequestValidator : AbstractValidator<Request>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Address)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);
        }
    }
}