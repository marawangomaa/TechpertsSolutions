using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class CustomerMapper
    {
        public static CustomerEditDTO MapToCustomerEditDTO(Customer customer)
        {
            var user = customer.User;
            var role = customer.Role;

            return new CustomerEditDTO
            {
                City = customer.City,
                Country = customer.Country,
            };
        }

        public static CustomerDTO MapToCustomerDTO(Customer customer)
        {
            var user = customer.User;
            var role = customer.Role;

            return new CustomerDTO
            {
                Id = customer.Id,
                City = customer.City,
                Country = customer.Country,

                // From IdentityUser (AppUser)
                Email = user?.Email,
                UserName = user?.UserName,
                PhoneNumber = user?.PhoneNumber,
                FullName = user?.FullName,
                Address = user?.Address,

                // From IdentityRole (AppRole)
                CartId = customer.Cart?.Id,
                WishListId = customer.WishList?.Id,
                PCAssemblyIds = customer.PCAssembly?.Select(p => p.Id).ToList(),
                OrderIds = customer.Orders?.Select(o => o.Id).ToList(),
                MaintenanceIds = customer.Maintenances?.Select(m => m.Id).ToList(),
                DeliveryId = customer.Delivery?.Id
            };
        }
    }
}
