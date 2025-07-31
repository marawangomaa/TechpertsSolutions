using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Repository.Data;

namespace Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<WishList> _wishListRepo;
        private readonly TechpertsContext context;

        public CustomerService(IRepository<Customer> customerRepository, 
            IRepository<Cart> _cartRepo, 
            IRepository<WishList> wishListRepo,
            TechpertsContext _context)
        {
            _customerRepository = customerRepository;
            cartRepo = _cartRepo;
            _wishListRepo = wishListRepo;
            context = _context;
        }

        public async Task<GeneralResponse<IEnumerable<CustomerDTO>>> GetAllCustomersAsync()
        {
            try
            {
                // Optimized includes for customer listing with essential related data
                var customers = await _customerRepository.GetAllWithIncludesAsync(
                    c => c.User, 
                    c => c.Role,
                    c => c.Cart,
                    c => c.WishList);

                var customerDtos = customers.Select(CustomerMapper.MapToCustomerDTO).ToList();

                return new GeneralResponse<IEnumerable<CustomerDTO>>
                {
                    Success = true,
                    Message = "Customers retrieved successfully.",
                    Data = customerDtos
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

        public async Task<GeneralResponse<CustomerEditDTO>> UpdateCustomerAsync(string id, CustomerEditDTO dto)
        {
            
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

                customer.User.City = dto.City;
                customer.User.Country = dto.Country;

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
                    City = customer.User.City,
                    Country = customer.User.Country,
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
            catch (Exception)
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

                
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    
                    var carts = await cartRepo.GetAllAsync();
                    var customerCarts = carts.Where(ct => ct.CustomerId == customer.Id).ToList();
                    foreach (var cart in customerCarts)
                    {
                        cartRepo.Remove(cart);
                    }

                    
                    if (_wishListRepo != null)
                    {
                        var wishLists = await _wishListRepo.FindAsync(w => w.CustomerId == customer.Id);
                        foreach (var wishList in wishLists)
                        {
                            _wishListRepo.Remove(wishList);
                        }
                    }

                    
                    var orders = await context.Orders.Where(o => o.CustomerId == customer.Id).ToListAsync();
                    foreach (var order in orders)
                    {
                        
                        context.Orders.Remove(order);
                    }

                    
                    var pcAssemblies = await context.PCAssemblies.Where(pca => pca.CustomerId == customer.Id).ToListAsync();
                    foreach (var pcAssembly in pcAssemblies)
                    {
                        
                        context.PCAssemblies.Remove(pcAssembly);
                    }

                    
                    var maintenances = await context.Maintenances.Where(m => m.CustomerId == customer.Id).ToListAsync();
                    foreach (var maintenance in maintenances)
                    {
                        
                        context.Maintenances.Remove(maintenance);
                    }

                    
                    var deliveries = await context.Deliveries.Where(d => d.CustomerId == customer.Id).ToListAsync();
                    foreach (var delivery in deliveries)
                    {
                        context.Deliveries.Remove(delivery);
                    }

                    
                    _customerRepository.Remove(customer);

                    
                    await context.SaveChangesAsync();
                    
                    
                    await transaction.CommitAsync();

                    return new GeneralResponse<bool>
                    {
                        Success = true,
                        Message = "Customer data cleaned up successfully.",
                        Data = true
                    };
                }
                catch (Exception)
                {
                    
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception)
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
                // Comprehensive includes for detailed customer view
                var customer = await _customerRepository.GetByIdWithIncludesAsync(id, 
                    c => c.User, 
                    c => c.Role,
                    c => c.Cart,
                    c => c.WishList,
                    c => c.Orders,
                    c => c.Maintenances,
                    c => c.PCAssembly);

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
    }
}
