using Core.Enums;
using Core.Interfaces;
using Core.Utilities;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedServiceUsages
    {
        public static async Task SeedServiceUsagesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var serviceUsageRepo = scope.ServiceProvider.GetRequiredService<IRepository<ServiceUsage>>();

            foreach (ServiceType serviceType in Enum.GetValues(typeof(ServiceType)))
            {
                string name = serviceType.GetStringValue();

                bool exists = await serviceUsageRepo.AnyAsync(su => su.ServiceType == name);
                if (!exists)
                {
                    var serviceUsage = new ServiceUsage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ServiceType = name,
                        CallCount = 0,
                        UsedOn = DateTime.UtcNow,
                        MaintenanceId = null,
                        Maintenance = null,
                        Orders = new List<Order>(),
                        PCAssemblies = new List<PCAssembly>()
                    };

                    await serviceUsageRepo.AddAsync(serviceUsage);
                }
            }

            await serviceUsageRepo.SaveChangesAsync();
        }
    }
}