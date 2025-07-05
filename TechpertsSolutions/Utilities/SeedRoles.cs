using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedRoles
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

            string[] roles = { "Admin", "Customer", "SaleManager", "StockControlManager", "TechCompany", "TechManager" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }
        }
    }
}
