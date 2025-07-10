using Core.Interfaces;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Core.DTOs.Customer;

namespace Service
{
    public class CustomerService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(MapToCustomerDTO).ToList();
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : MapToCustomerDTO(customer);
        }

        private CustomerDTO MapToCustomerDTO(Customer customer)
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
                EmailConfirmed = user?.EmailConfirmed ?? false,
                FullName = user?.FullName,
                Address = user?.Address,

                // From IdentityRole (AppRole)
                RoleName = role?.Name,
                RoleNotes = role?.Notes,

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
