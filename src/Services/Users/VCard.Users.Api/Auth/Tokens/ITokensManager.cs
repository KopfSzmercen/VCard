using System.Security.Claims;

namespace VCard.Users.Api.Auth.Tokens;

public interface ITokensManager
{
    JsonWebToken CreateToken(Guid userId, List<string> roles, string audience = null, List<Claim>? claims = null);
}