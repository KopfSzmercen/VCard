using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VCard.Users.Api.Auth;

internal class UserRole : IdentityRole<Guid>
{
    public const string Admin = "Admin";
    public const string User = "User";

    public UserRole(string name) : base(name)
    {
        Name = name;
    }

    public string Name { get; }

    public static IReadOnlyList<string> AvailableRoles =>
    [
        Admin,
        User
    ];
}

internal sealed class UserRolesInitializer(IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();

        foreach (var role in UserRole.AvailableRoles)
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new UserRole(role));

        var allRoles
            = await roleManager.Roles.ToListAsync(cancellationToken);

        foreach (
            var role in allRoles.Where(role => !UserRole.AvailableRoles.Contains(role.Name))
        )
            await roleManager.DeleteAsync(role);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}