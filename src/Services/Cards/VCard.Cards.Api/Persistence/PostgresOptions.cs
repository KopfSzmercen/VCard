namespace VCard.Cards.Api.Persistence;

public class PostgresOptions
{
    public string ConnectionString { get; set; }

    public const string SectionName = "Postgres";
}