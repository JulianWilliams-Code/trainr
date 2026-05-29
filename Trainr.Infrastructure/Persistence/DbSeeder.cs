using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Trainr.Domain.Enums;
using Trainr.Infrastructure.Identity;

namespace Trainr.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = [UserRole.Admin, UserRole.Trainer, UserRole.Client];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    public static async Task SeedAdminAsync(IServiceProvider services, string adminEmail, string adminPassword)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName  = adminEmail,
            Email     = adminEmail,
            FirstName = "Admin",
            LastName  = "User",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, UserRole.Admin);
    }
}
