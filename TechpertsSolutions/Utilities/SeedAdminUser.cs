using Core.Enums;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace TechpertsSolutions.Utilities
{
    public static class SeedAdminUser
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Check if admin user already exists
            var adminEmail = "admin@techperts.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                // Create admin user
                var adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    Address = "System Address",
                    ProfilePhotoUrl = "profiles/default-profile.jpg",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Assign admin role
                    var adminRole = RoleType.Admin.GetStringValue();
                    if (await roleManager.RoleExistsAsync(adminRole))
                    {
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }

                    // Create Admin entity
                    var adminEntity = new Admin
                    {
                        Id = adminUser.Id,
                        UserId = adminUser.Id,
                        RoleId = (await roleManager.FindByNameAsync(adminRole))?.Id
                    };

                    var context = serviceProvider.GetRequiredService<TechpertsContext>();
                    context.Admins.Add(adminEntity);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"Admin user created successfully: {adminEmail}");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Admin user already exists: {adminEmail}");
            }
        }
    }
} 
