using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VCard.Common.Application.RequestContext;
using VCard.Common.Presentation.Endpoints;
using VCard.Communication.Api.EmailSender;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Api.Presentation;

internal sealed class SendEmailEndpoint : IEndpoint
{
    internal sealed class Client(
        HttpClient httpClient
    )
    {
        public async Task<Response?> GetUserAccountAsync(Guid userId)
        {
            var response = await httpClient.GetAsync(
                $"users/{userId}/account"
            );

            var responseContent = await response.Content.ReadFromJsonAsync<Response?>();

            return responseContent;
        }

        public sealed record Response(string Username, string Address);
    }

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("", Handle)
            .WithSummary("Send an email")
            .RequireAuthorization("SendEmails");
    }

    private static async Task<Results<
        Ok,
        BadRequest<string>
    >> Handle(
        [FromServices] IEmailSender emailSender,
        [FromServices] IRequestContext requestContext,
        [FromServices] Client client,
        [FromBody] Request request,
        [FromServices] IBus bus
    )
    {
        //Simulate that I need current user data to send the email - I have to communicate with another service
        var userAccount = await client.GetUserAccountAsync((Guid)requestContext.Id!);

        if (userAccount is null)
            return TypedResults.BadRequest("User account not found");

        await emailSender.SendEmailAsync(
            request.To,
            request.Subject,
            request.Body
        );

        await bus.Publish(new EmailSent(
            Guid.NewGuid(),
            requestContext.Id!.Value
        ));

        return TypedResults.Ok();
    }

    public sealed record Request
    {
        public required string To { get; init; }
        public required string Subject { get; init; }
        public required string Body { get; init; }
    }
}