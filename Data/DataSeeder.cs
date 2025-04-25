using FinanceManagement.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManagement.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure roles exist
            string[] roleNames = { "Admin", "DepartementManger", "Financier", "Employe" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
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
                    PhoneNumber = "1234567890"
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
        }
    }
}