using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using TechpertsSolutions.Core.DTOs.Customer;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Cart> cartRepo;

        public CustomerService(IRepository<Customer> customerRepository,IRepository<Cart> _cartRepo)
        {
            _customerRepository = customerRepository;
            cartRepo = _cartRepo;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllWithIncludesAsync(c=>c.User);
            return customers.Select(CustomerMapper.MapToCustomerDTO).ToList();
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsyn(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : CustomerMapper.MapToCustomerDTO(customer);
        }
        public async Task<CustomerEditDTO?> UpdateCustomerAsync(string id, CustomerEditDTO dto)
        {
            var customer = await _customerRepository.GetByIdWithIncludesAsync(id, c => ((Customer)c).User);
            if (customer == null) return null;

            customer.City = dto.City;
            customer.Country = dto.Country;

            if (customer.User != null)
            {
                customer.User.FullName = dto.FullName;
                customer.User.Email = dto.Email;
                customer.User.PhoneNumber = dto.PhoneNumber;
                customer.User.UserName = dto.UserName;
                customer.User.Address = dto.Address;
            }

            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return new CustomerEditDTO
            {
                City = customer.City,
                Country = customer.Country,
                FullName = customer.User?.FullName,
                Email = customer.User?.Email,
                PhoneNumber = customer.User?.PhoneNumber,
                UserName = customer.User?.UserName,
                Address = customer.User?.Address
            };
        }

        public async Task CleanupCustomerDataAsync(string userId)
        {
            var customer = (await _customerRepository.GetAllAsync()).FirstOrDefault(c => c.UserId == userId);
            if (customer != null)
            {
                _customerRepository.Remove(customer);

                var carts = await cartRepo.GetAllAsync();
                var customerCarts = carts.Where(ct => ct.CustomerId == customer.Id).ToList();
                foreach (var cart in customerCarts)
                    cartRepo.Remove(cart);
            }
        }
        public async Task<CustomerDTO?> GetCustomerByIdAsync(string id)
        {
            var customer = await _customerRepository.GetByIdWithIncludesAsync(id, c => c.User);
            if (customer == null) return null;

            return new CustomerDTO
            {
                Id = customer.Id,
                City = customer.City,
                Country = customer.Country,
                Email = customer.User?.Email,
                UserName = customer.User?.UserName,
                FullName = customer.User?.FullName,
                PhoneNumber = customer.User?.PhoneNumber,
                Address = customer.User?.Address
            };
        }
    }
}
