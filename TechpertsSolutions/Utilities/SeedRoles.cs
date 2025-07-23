using Core.Enums;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedRoles
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

            foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
            {
                var roleName = role.GetStringValue();

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }
        }
    }
}
