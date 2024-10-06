namespace VCard.Cards.Api.Events;

public interface IEventHandler
{
    Task HandleAsync(object eventEnvelope, CancellationToken ct);
}

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent eventEnvelope, CancellationToken ct);
}

public class EventHandler<TEvent>(Func<TEvent, CancellationToken, Task> handler) : IEventHandler<TEvent>
{
    public Task HandleAsync(TEvent eventEnvelope, CancellationToken ct)
    {
        return handler(eventEnvelope, ct);
    }
}