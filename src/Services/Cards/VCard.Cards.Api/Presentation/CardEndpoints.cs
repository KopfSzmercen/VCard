using VCard.Common.Presentation.Endpoints;

namespace VCard.Cards.Api.Presentation;

internal static class CardEndpoints
{
    private const string BasePath = "cards";
    private const string Tag = "Cards";

    internal static void MapCardEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup(BasePath)
            .WithTags(Tag);

        group
            .MapEndpoint<GetCardEndpoint>()
            .MapEndpoint<AddMoneyEndpoint>()
            .MapEndpoint<WithdrawMoneyEndpoint>();
    }
}