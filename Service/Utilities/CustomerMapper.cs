using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class CustomerMapper
    {
        public static CustomerEditDTO? MapToCustomerEditDTO(Customer? customer)
        {
            if (customer == null) return null;
            
            var user = customer.User;
            var role = customer.Role;

            return new CustomerEditDTO
            {
                City = customer.User.City,
                Country = customer.User.Country,
            };
        }

        public static CustomerDTO? MapToCustomerDTO(Customer? customer)
        {
            if (customer == null) return null;
            
            var user = customer.User;
            var role = customer.Role;

            return new CustomerDTO
            {
                Id = customer.Id,
                City = customer.User.City,
                Country = customer.User.Country,
                Email = user?.Email,
                UserName = user?.UserName,
                PhoneNumber = user?.PhoneNumber,
                FullName = user?.FullName,
                Address = user?.Address,
                PostalCode = user?.PostalCode,
                Latitude = user?.Latitude,
                Longitude = user?.Longitude,
                CartId = customer.Cart?.Id,
                WishListId = customer.WishList?.Id,
                DeliveryIds = customer.Deliveries?.Select(d => d.Id).ToList(),
                PCAssemblyIds = customer.PCAssembly?.Select(p => p.Id).ToList(),
                OrderIds = customer.Orders?.Select(o => o.Id).ToList(),
                MaintenanceIds = customer.Maintenances?.Select(m => m.Id).ToList()
            };
        }
    }
}
