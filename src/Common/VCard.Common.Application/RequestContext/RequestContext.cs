using System.Security.Claims;

namespace VCard.Common.Application.RequestContext;

public sealed class RequestContext : IRequestContext
{
    public RequestContext(HttpContext httpContext)
    {
        IsAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;

        Id = IsAuthenticated ? Guid.Parse(httpContext!.User!.Claims.First(x => x.Type == "UserId").Value) : null;

        Roles = IsAuthenticated
            ? httpContext!.User.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList()
            : [];

        Claims = IsAuthenticated
            ? httpContext!.User.Claims
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x =>
                    x.Select(c => c.Value.ToString())
                )
            : null;
    }

    public Guid? Id { get; }

    public bool IsAuthenticated { get; }

    public List<string> Roles { get; }

    public Dictionary<string, IEnumerable<string>>? Claims { get; }
}