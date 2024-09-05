using Microsoft.AspNetCore.Routing;

namespace VCard.Common.Presentation.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}