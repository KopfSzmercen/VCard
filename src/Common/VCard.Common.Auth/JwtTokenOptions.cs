namespace VCard.Common.Auth;

public sealed class JwtTokensOptions
{
    public const string SectionName = "Jwt";

    public string SigningKey { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public string Issuer { get; init; } = null!;
}