using Microsoft.AspNetCore.Routing;

namespace VCard.Common.Presentation.Endpoints;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}