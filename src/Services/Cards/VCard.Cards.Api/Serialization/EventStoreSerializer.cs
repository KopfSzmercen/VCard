using System.Reflection;
using System.Text;
using EventStore.Client;
using Newtonsoft.Json;

namespace VCard.Cards.Api.Serialization;

internal static class EventStoreSerializer
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };

    public static object? Deserialize(this ResolvedEvent resolvedEvent)
    {
        var eventType = ToType(resolvedEvent.Event.EventType);

        return JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
            eventType,
            JsonSerializerSettings
        );
    }

    private static Type ToType(string eventType)
    {
        var type = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(x => x.Name == eventType || x.FullName == eventType);

        if (type is null)
            throw new InvalidOperationException($"Type {eventType} not found in current assembly");

        return type;
    }

    public static EventData ToJsonEventData(this object @event)
    {
        return new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event))
        );
    }
}