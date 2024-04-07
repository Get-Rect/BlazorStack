using BlazorStack.Data.Contexts;
using BlazorStack.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Data
{
    public static class Seeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var rolesManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            string defaultAdminEmail = config["Seeder:DefaultAdminEmail"] ?? throw new Exception("Default admin email not configured.");

            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                string[] roles = new string[] { "Admin" };

                var newrolelist = new List<IdentityRole>();
                foreach (string role in roles)
                {
                    var dbRole = rolesManager.Roles.FirstOrDefault(x => x.Name == role);
                    if (dbRole == null) await rolesManager.CreateAsync(new IdentityRole { Name = role });
                }

                if (!context.Users.Any(x => defaultAdminEmail.Equals(x.Email)))
                {        
                    var userResult = await userManager.CreateAsync(new ApplicationUser() { Email = defaultAdminEmail, UserName = defaultAdminEmail }, "Test123!");
                    if (!userResult.Succeeded) throw new Exception("Failed to create default admin user.");
                }

                var defaultAdmin = await userManager.FindByEmailAsync(defaultAdminEmail) ?? throw new Exception("Error seeding admin user");
                if (!await userManager.IsInRoleAsync(defaultAdmin, "Admin"))
                {
                    var roleResult = await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                    if (!roleResult.Succeeded) throw new Exception("Failed to add admin role to default admin user in seeder.");
                }
            }
        }
    }
}
