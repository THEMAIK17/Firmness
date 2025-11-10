using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Firmness.Web.DataSeeder;

public static class IdentitySeeder
    {
        // Define the role names required by TASK 4
        private const string AdminRole = "Administrator";
        private const string ClientRole = "Client";
        
        // Define the default admin user credentials
        private const string DefaultAdminEmail = "admin@firmeza.com";
        private const string DefaultAdminPassword = "AdminPassword123*"; 

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Client>>();

            //  Create Roles if they do not exist
            await EnsureRoleExists(roleManager, AdminRole);
            await EnsureRoleExists(roleManager, ClientRole);

            //  Create the default Administrator user
            var adminUser = await userManager.FindByEmailAsync(DefaultAdminEmail);
            if (adminUser == null)
            {
                adminUser = new Client
                {
                    UserName = DefaultAdminEmail,
                    Email = DefaultAdminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    DocumentNumber = "000000000000" 
                };

                var result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);
                if (result.Succeeded)
                {
                    // Assign the Administrator role to the new user
                    await userManager.AddToRoleAsync(adminUser, AdminRole);
                }
            }
        }

        private static async Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }