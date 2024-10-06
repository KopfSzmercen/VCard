using Newtonsoft.Json;

namespace VCard.Cards.Api.Cards;

internal sealed record Money
{
    private Money()
    {
    }

    [JsonConstructor]
    public Money(int amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        if (amount < 0)
            throw new ArgumentException("Amount must be greater than or equal 0", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public int Amount { get; private init; }
    public string Currency { get; private init; }

    public static IReadOnlyList<string> AvailableCurrencies => ["PLN", "USD"];
}