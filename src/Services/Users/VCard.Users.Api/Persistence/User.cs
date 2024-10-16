using Microsoft.AspNetCore.Identity;

namespace VCard.Users.Api.Persistence;

internal sealed class User : IdentityUser<Guid>
{
    public UserAccount? UserAccount { get; set; }
}