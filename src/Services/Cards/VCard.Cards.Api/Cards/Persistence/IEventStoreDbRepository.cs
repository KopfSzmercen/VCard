namespace VCard.Cards.Api.Cards.Persistence;

public interface IEventStoreDbRepository<TEntity> where TEntity : notnull
{
    Task<TEntity?> FindAsync(Func<TEntity?, object, TEntity> when, string id, CancellationToken cancellationToken);

    Task AppendAsync(string id, object @event, CancellationToken cancellationToken);

    Task AppendAsync(string id, object @event, uint version, CancellationToken cancellationToken);
}