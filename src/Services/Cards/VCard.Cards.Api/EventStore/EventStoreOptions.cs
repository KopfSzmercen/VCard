namespace VCard.Cards.Api.EventStore;

public class EventStoreOptions
{
    public const string SectionName = "EventStore";

    public string ConnectionString { get; set; } = string.Empty;
}