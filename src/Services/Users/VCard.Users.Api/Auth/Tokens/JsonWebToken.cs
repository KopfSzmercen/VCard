namespace VCard.Users.Api.Auth.Tokens;

public class JsonWebToken
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public long Expiry { get; set; }

    public Guid UserId { get; set; }
}