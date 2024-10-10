using System.Security.Claims;


namespace VCard.Common.Auth.TokensManager;

public interface ITokensManager
{
    JsonWebToken CreateToken(Guid userId, List<string> roles, List<Claim>? claims = null);
}

public class JsonWebToken
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public long Expiry { get; set; }

    public Guid UserId { get; set; }
}