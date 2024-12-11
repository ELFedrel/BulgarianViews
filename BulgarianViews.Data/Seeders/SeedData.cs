using BulgarianViews.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace BulgarianViews.Data.Seeders
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roleNames = { "Admin", "User" };

           
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                }
            }

            
            var adminEmail = "admin@domain.com";
            var adminPassword = "Admin@123"; 
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
