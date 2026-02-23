using Microsoft.AspNetCore.Identity;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Nomes das roles
            string[] roleNames = { "Administrador", "Vendedor", "Comprador" };

            foreach (var roleName in roleNames)
            {
                
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminUserAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {

            var adminUsername = configuration["AdminAccountDefaults:Username"];
            var adminEmail = configuration["AdminAccountDefaults:Email"];
            var adminPassword = configuration["AdminAccountDefaults:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                return;
            }

            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                User adminUser = new User
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                   
                    var adminEntry = new Administrador
                    {
                        IdUtilizador = adminUser.Id,
                    };

                    context.Administradores.Add(adminEntry);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
