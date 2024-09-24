using Microsoft.Extensions.Options;

namespace VCard.Users.Api.Auth.Tokens;

internal sealed class JwtTokensOptions
{
    public const string SectionName = "Jwt";

    public string SigningKey { get; set; }

    public string Audience { get; set; }

    public string Issuer { get; set; }
}

internal sealed class JwtTokensOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtTokensOptions>
{
    public void Configure(JwtTokensOptions options)
    {
        configuration
            .GetSection(JwtTokensOptions.SectionName)
            .Bind(options);
    }
}