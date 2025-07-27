using Core.Enums;
using Core.Utilities;

namespace TechpertsSolutions.Utilities
{
    public static class SeedEnums
    {
        public static void LogEnumValues()
        {
            Console.WriteLine("=== Enum Values Available ===");
            
            // ProductPendingStatus
            Console.WriteLine("\nProductPendingStatus Values:");
            foreach (ProductPendingStatus status in Enum.GetValues(typeof(ProductPendingStatus)))
            {
                Console.WriteLine($"  {status} = {status.GetStringValue()}");
            }

            // ServiceType
            Console.WriteLine("\nServiceType Values:");
            foreach (ServiceType serviceType in Enum.GetValues(typeof(ServiceType)))
            {
                Console.WriteLine($"  {serviceType} = {serviceType.GetStringValue()}");
            }

            // ProductCategory
            Console.WriteLine("\nProductCategory Values:");
            foreach (ProductCategory category in Enum.GetValues(typeof(ProductCategory)))
            {
                Console.WriteLine($"  {category} = {category.GetStringValue()}");
            }

            // RoleType
            Console.WriteLine("\nRoleType Values:");
            foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
            {
                Console.WriteLine($"  {role} = {role.GetStringValue()}");
            }

            Console.WriteLine("=== Enum Seeding Completed ===");
        }
    }
} 