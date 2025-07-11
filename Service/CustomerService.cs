using Core.Interfaces;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Core.DTOs.Customer;
using Service.Utilities;

namespace Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(CustomerMapper.MapToCustomerDTO).ToList();
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : CustomerMapper.MapToCustomerDTO(customer);
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

            return CustomerMapper.MapToCustomerEditDTO(customer);
        }
    }
}
