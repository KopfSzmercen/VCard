using MassTransit;
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
    public DbSet<UserAccount> UserAccount { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("users");

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();

        ConfigureUserAccount(builder);
    }

    private static void ConfigureUserAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithOne(e => e.UserAccount)
                .HasForeignKey<UserAccount>(e => e.UserId);
        });
    }
}