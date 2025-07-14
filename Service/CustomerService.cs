using Core.Interfaces;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Core.DTOs.Customer;
using Service.Utilities;

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
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(CustomerMapper.MapToCustomerDTO).ToList();
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsyn(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : CustomerMapper.MapToCustomerDTO(customer);
        }
        public async Task<CustomerEditDTO?> UpdateCustomerAsync(string id, CustomerEditDTO dto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            // Update scalar properties
            customer.City = dto.City;
            customer.Country = dto.Country;

            _customerRepository.Update(customer);
            await _customerRepository.SaveChanges();

            return CustomerMapper.MapToCustomerEditDTO(customer);
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
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : CustomerMapper.MapToCustomerDTO(customer);
        }
    }
}
