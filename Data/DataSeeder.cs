using FinanceManagement.DbSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Data;

public static class DataSeeder
{
    // Define fixed role IDs to prevent remapping on migrations
    private static readonly Dictionary<string, string> RoleIds = new()
    {
        ["Admin"] = "1d4e29f7-ac05-4d96-9ab1-9cea3d4d9505",
        ["DepartementManager"] = "0c3d1885-2e2a-4946-b9d6-cc2aa5d7a8ef",
        ["Financier"] = "bdff7291-c42f-4c76-bc3f-fd9288a25c63",
        ["Employe"] = "6f85aa6b-3e3c-4519-af4c-037a53d20896"
    };

    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure roles exist with fixed IDs
        foreach (var roleEntry in RoleIds)
        {
            var roleName = roleEntry.Key;
            var roleId = roleEntry.Value;

            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var role = new IdentityRole
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                await roleManager.CreateAsync(role);
            }
        }

        // Seed Admin user
        string adminEmail = "admin@financemanagement.com";
        string adminUsername = "admin";
        string adminPassword = "Admin@1234"; // Must meet password validation criteria

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Utilisateur
            {
                UserName = adminUsername,
                Email = adminEmail,
                Nom = "Admin",
                Prenom = "User",
                Addresse = "Admin Address",
                Cin = "ADMIN123",
                PhoneNumber = "1234567890",
                DateEmbauche = DateTime.Now,
                DerniereConnexion = DateTime.Now,
                Status = true,
                IdDepartement = 1, // Ensure a Department with IdDepartement = 1 exists
                EmailConfirmed = true // Since you require confirmed email in your Identity options
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception("Failed to create Admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Ensure the admin user has the Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}