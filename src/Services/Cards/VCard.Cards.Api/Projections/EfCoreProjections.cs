using Microsoft.EntityFrameworkCore;
using VCard.Cards.Api.Events;

namespace VCard.Cards.Api.Projections;

public class AddProjection<TView, TEvent, TDbContext>(
    TDbContext dbContext,
    Func<EventEnvelope<TEvent>, TView> create)
    : IEventHandler<EventEnvelope<TEvent>> where TDbContext : DbContext where TView : class where TEvent : notnull
{
    public async Task HandleAsync(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var view = create(eventEnvelope);

        await dbContext.AddAsync(view, ct);

        await dbContext.SaveChangesAsync(ct);
    }
}

public class UpdateProjection<TView, TEvent, TDbContext>(
    TDbContext dbContext,
    Func<TEvent, object> getViewId,
    Action<EventEnvelope<TEvent>, TView> update)
    : IEventHandler<EventEnvelope<TEvent>> where TDbContext : DbContext where TView : class where TEvent : notnull
{
    public async Task HandleAsync(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var viewId = getViewId(eventEnvelope.Data);
        var view = await dbContext.FindAsync<TView>([viewId], ct);

        if (view == null)
        {
            Console.WriteLine($"View not found: {viewId} for event {eventEnvelope.Data.GetType()}");
            return;
        }

        update(eventEnvelope, view);

        await dbContext.SaveChangesAsync(ct);
    }
}