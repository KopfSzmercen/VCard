using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VCard.Users.Api.Auth.Tokens;

internal sealed class TokensManager : ITokensManager
{
    public const string Secret =
        "9c6e116755e3d00729e8746e064b28906208001b75c7cefa19527eb4fa05a29e1cabbb57ea681d75c83da27fcb93c94763730ea47cc8fa4985930bb886185984ab93a8ea762402a06cd4cc0ff303306cff6948c5e53501d177661395d2c54aff943d79576b06fdeade1b95aaed093fd5203b35bb5f36f2d0e860c7ec496889531c41f58192934d94a5dc8c554fd60713eddf8c2b33860e7e828f6ea689e57607f16047bcd614b136891ddf11a85cbb2025681981578aa00c58ece9eace8b8ccb8acd8fa2fd7ed5e889b25afc493bfc9420a3e39774261255e2513d7e145d46dd6c49e9ebb5da4684cef53ae19064f7aca7fcc3eaf5034a463bf908430865d0cb";

    public JsonWebToken CreateToken(
        Guid userId,
        List<string> roles,
        string audience = null,
        List<Claim>? claims = null
    )
    {
        var now = DateTimeOffset.UtcNow;

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        jwtClaims.AddRange(roles
            .Select(role => new Claim(ClaimTypes.Role, role))
        );

        var jwt = new JwtSecurityToken(
            claims: jwtClaims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(30).UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret)),
                SecurityAlgorithms.HmacSha256
            ),
            audience: "VCard.Users"
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