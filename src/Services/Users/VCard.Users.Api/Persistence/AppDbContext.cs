using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VCard.Users.Api.Auth;

namespace VCard.Users.Api.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) :
    IdentityDbContext<
        User,
        UserRole,
        Guid
    >(options), IDataProtectionKeyContext


{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("users");
    }
}