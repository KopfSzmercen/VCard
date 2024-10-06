using VCard.Cards.Api.Persistence;
using VCard.Cards.Api.Projections;

namespace VCard.Cards.Api.EventStore.Checkpoints;

public interface ICheckpointRepository
{
    public Task<Checkpoint?> GetLastCheckpointAsync(string subscriptionId, CancellationToken cancellationToken);

    public Task SaveCheckpointAsync(Checkpoint checkpoint, CancellationToken cancellationToken);

    public Task UpdateCheckpointAsync(Checkpoint checkpoint, CancellationToken cancellationToken);
}

internal sealed class PostgresCheckpointRepository(AppDbContext dbContext) : ICheckpointRepository
{
    public async Task<Checkpoint?> GetLastCheckpointAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        return await dbContext.Checkpoints.FindAsync([subscriptionId], cancellationToken);
    }

    public async Task SaveCheckpointAsync(Checkpoint checkpoint, CancellationToken cancellationToken)
    {
        dbContext.Checkpoints.Add(checkpoint);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCheckpointAsync(Checkpoint checkpoint, CancellationToken cancellationToken)
    {
        dbContext.Checkpoints.Update(checkpoint);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}