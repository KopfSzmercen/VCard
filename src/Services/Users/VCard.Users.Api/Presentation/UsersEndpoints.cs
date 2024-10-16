using VCard.Common.Presentation.Endpoints;

namespace VCard.Users.Api.Presentation;

internal static class UsersEndpoints
{
    public const string BaseUrl = "users";
    public const string GroupTag = "users";

    public static void AddUsersEndpoint(this WebApplication webApplication)
    {
        var group = webApplication
            .MapGroup(BaseUrl)
            .WithTags(GroupTag);

        group
            .MapEndpoint<RegisterUserEndpoint>()
            .MapEndpoint<SignInUserEndpoint>()
            .MapEndpoint<GetMeEndpoint>()
            .MapEndpoint<SignInUserJwtEndpoint>()
            .MapEndpoint<UpdateAccountEndpoint>()
            .MapEndpoint<GetUserAccountEndpoint>();
    }
}