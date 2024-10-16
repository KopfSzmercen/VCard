namespace VCard.Users.Api.Persistence;

internal sealed class UserAccount
{
    public Guid Id { get; set; }

    public required User User { get; set; }

    public Guid UserId { get; set; }

    public required string Username { get; set; }

    public required string Address { get; set; }
}