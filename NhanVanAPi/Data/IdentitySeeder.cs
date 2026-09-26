using Microsoft.AspNetCore.Identity;
using NhanVanAPi.Models;

namespace NhanVanAPi.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync (IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            //create admin role
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

           // Define the account
            var email = config["AdminUser:Email"]!;
            var password = config["AdminUser:Password"]!;
            var fullName = config["AdminUser:FullName"]!;

            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin,password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");

        }
    }
}
