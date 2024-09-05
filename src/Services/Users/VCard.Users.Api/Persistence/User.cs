using Microsoft.AspNetCore.Identity;

namespace VCard.Users.Api.Persistence;

internal sealed class User : IdentityUser<Guid>
{
}