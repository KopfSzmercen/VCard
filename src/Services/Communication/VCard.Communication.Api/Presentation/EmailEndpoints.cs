using VCard.Common.Presentation.Endpoints;

namespace VCard.Communication.Api.Presentation;

internal static class EmailEndpoints
{
    public const string BasePath = "v1/emails";
    public const string GroupTag = "Emails";

    public static void MapEmailEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup(BasePath)
            .WithTags(GroupTag);

        group.MapEndpoint<SendEmailEndpoint>();
    }
}