namespace VCard.Cards.Api.EventStore.Checkpoints;

public record Checkpoint
{
    public string SubscriptionId { get; init; }
    public ulong Position { get; private set; }
    public DateTime CheckpointedAt { get; private set; }

    private Checkpoint()
    {
    }

    public Checkpoint(string subscriptionId, ulong position, DateTime createdAt)
    {
        SubscriptionId = subscriptionId;
        Position = position;
        CheckpointedAt = createdAt;
    }

    public void MakeCheckpoint(ulong position, DateTime checkpointedAt)
    {
        Position = position;
        CheckpointedAt = checkpointedAt;
    }
};