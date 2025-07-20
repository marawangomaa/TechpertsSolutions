using Core.Interfaces;
using Core.Interfaces.Services;
using TechpertsSolutions.Core.DTOs;
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

        public async Task<GeneralResponse<IEnumerable<CustomerDTO>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllWithIncludesAsync(c=>c.User);
                return new GeneralResponse<IEnumerable<CustomerDTO>>
                {
                    Success = true,
                    Message = "Customers retrieved successfully.",
                    Data = customers.Select(CustomerMapper.MapToCustomerDTO).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<CustomerDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving customers.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CustomerDTO>> GetCustomerByIdAsyn(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return new GeneralResponse<CustomerDTO>
                    {
                        Success = false,
                        Message = $"Customer with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = CustomerMapper.MapToCustomerDTO(customer)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the customer.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CustomerEditDTO>> UpdateCustomerAsync(string id, CustomerEditDTO dto)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (dto == null)
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "Full name is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "Email is required.",
                    Data = null
                };
            }

            try
            {
                var customer = await _customerRepository.GetByIdWithIncludesAsync(id, c => ((Customer)c).User);
                if (customer == null)
                {
                    return new GeneralResponse<CustomerEditDTO>
                    {
                        Success = false,
                        Message = $"Customer with ID '{id}' not found.",
                        Data = null
                    };
                }

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

                var updatedCustomer = new CustomerEditDTO
                {
                    City = customer.City,
                    Country = customer.Country,
                    FullName = customer.User?.FullName,
                    Email = customer.User?.Email,
                    PhoneNumber = customer.User?.PhoneNumber,
                    UserName = customer.User?.UserName,
                    Address = customer.User?.Address
                };

                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = true,
                    Message = "Customer updated successfully.",
                    Data = updatedCustomer
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CustomerEditDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the customer.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> CleanupCustomerDataAsync(string userId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "User ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(userId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid User ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var customer = (await _customerRepository.GetAllAsync()).FirstOrDefault(c => c.UserId == userId);
                if (customer == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Customer with User ID '{userId}' not found.",
                        Data = false
                    };
                }

                _customerRepository.Remove(customer);

                var carts = await cartRepo.GetAllAsync();
                var customerCarts = carts.Where(ct => ct.CustomerId == customer.Id).ToList();
                foreach (var cart in customerCarts)
                    cartRepo.Remove(cart);

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Customer data cleaned up successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while cleaning up customer data.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<CustomerDTO>> GetCustomerByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var customer = await _customerRepository.GetByIdWithIncludesAsync(id, c => c.User);
                if (customer == null)
                {
                    return new GeneralResponse<CustomerDTO>
                    {
                        Success = false,
                        Message = $"Customer with ID '{id}' not found.",
                        Data = null
                    };
                }

                var customerDto = new CustomerDTO
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

                return new GeneralResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = customerDto
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CustomerDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the customer.",
                    Data = null
                };
            }
        }
    }
}
