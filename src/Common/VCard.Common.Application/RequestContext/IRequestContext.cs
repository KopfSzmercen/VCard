namespace VCard.Common.Application.RequestContext;

public interface IRequestContext
{
    public bool IsAuthenticated { get; }

    public Guid? Id { get; }

    public List<string> Roles { get; }

    Dictionary<string, IEnumerable<string>>? Claims { get; }
}