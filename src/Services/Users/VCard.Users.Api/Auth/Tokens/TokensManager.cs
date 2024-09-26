using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace VCard.Users.Api.Auth.Tokens;

internal sealed class TokensManager(
    IOptions<JwtTokensOptions> jwtOptions
) : ITokensManager
{
    public JsonWebToken CreateToken(
        Guid userId,
        List<string> roles,
        List<Claim>? claims = null
    )
    {
        var now = DateTimeOffset.UtcNow;

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("UserId", userId.ToString())
        };

        jwtClaims.AddRange(roles
            .Select(role => new Claim(ClaimTypes.Role, role))
        );

        if (claims != null) jwtClaims.AddRange(claims);

        var jwt = new JwtSecurityToken(
            claims: jwtClaims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(30).UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Value.SigningKey)),
                SecurityAlgorithms.HmacSha256
            ),
            audience: jwtOptions.Value.Audience,
            issuer: jwtOptions.Value.Issuer
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JsonWebToken
        {
            AccessToken = accessToken,
            RefreshToken = Guid.NewGuid().ToString(),
            Expiry = ((DateTimeOffset)jwt.ValidTo).ToUnixTimeSeconds(),
            UserId = userId
        };
    }
}