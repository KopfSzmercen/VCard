using System.Security.Claims;

namespace VCard.Users.Api.Auth.Tokens;

public interface ITokensManager
{
    JsonWebToken CreateToken(Guid userId, List<string> roles, List<Claim>? claims = null);
}