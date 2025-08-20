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
                bool exists = await serviceUsageRepo.AnyAsync(
                    su => su.ServiceType == ServiceType.Maintenance
                );
                if (!exists)
                {
                    var serviceUsage = new ServiceUsage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ServiceType = serviceType,
                        CallCount = 0,
                        UsedOn = DateTime.Now,
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