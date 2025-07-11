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
        public async Task<CustomerEditDTO?> UpdateCustomerAsync(int id, CustomerEditDTO dto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            // Update scalar properties
            customer.City = dto.City;
            customer.Country = dto.Country;

            // Update nested AppUser data
            if (customer.User != null)
            {
                customer.User.FullName = dto.FullName;
                customer.User.Email = dto.Email;
                customer.User.PhoneNumber = dto.PhoneNumber;
                customer.User.Address = dto.Address;
                customer.User.UserName = dto.UserName;
            }
            if (customer.Role != null)
                customer.Role.Notes = dto.RoleNotes;

            _customerRepository.Update(customer);
            await _customerRepository.SaveChanges();

            return MapToCustomerEditDTO(customer);
        }


        private CustomerEditDTO MapToCustomerEditDTO(Customer customer)
        {
            var user = customer.User;
            var role = customer.Role;

            return new CustomerEditDTO
            {
                City = customer.City,
                Country = customer.Country,

                // From IdentityUser (AppUser)
                Email = user?.Email,
                UserName = user?.UserName,
                PhoneNumber = user?.PhoneNumber,
                FullName = user?.FullName,
                Address = user?.Address,

                // From IdentityRole (AppRole)
                RoleName = role?.Name,
                RoleNotes = role?.Notes
            };
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
