using Microsoft.AspNetCore.Identity;

using Shortener.DAL.Entities;

namespace WebApp.DataAccess.Data
{
    public class DbInitializer
    {
        private readonly UserManager<ShortenerUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public DbInitializer(UserManager<ShortenerUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAdminAsync()
        {
            const string adminRole = "Admin";
            const string adminEmail = "admin@yourdomain.com";
            const string adminPassword = "Admin@1234"; // Specify a more secure password

            // Check if the administrator role exists
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to create administrator role.");
                }
            }

            // Check if the administrator user exists
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ShortenerUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create administrator: {string.Join(", ", createResult.Errors)}");
                }

                // Add the user to the administrator role
                var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, adminRole);
                if (!addToRoleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to add user to administrator role: {string.Join(", ", addToRoleResult.Errors)}");
                }
            }
        }

        public async Task SeedUserRoleAsync()
        {
            const string userRole = "User";

            // Перевіряємо, чи існує роль "User"
            if (!await _roleManager.RoleExistsAsync(userRole))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(userRole));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create {userRole} role.");
                }
            }
        }
    }
}