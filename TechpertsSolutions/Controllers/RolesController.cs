using Core.DTOs.RoleDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.RegisterDTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Core.DTOs;


namespace TechpertsSolutions.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRepository<Admin> adminRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<DeliveryPerson> deliveryPersonRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<WishList> wishListRepo;
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly TechpertsContext context;
        private readonly ICustomerService customerService;

        public RolesController(RoleManager<AppRole> _roleManager,
            UserManager<AppUser> _userManager,
            IRepository<Admin> _adminRepo,
            IRepository<Customer> _customerRepo,
            IRepository<TechCompany> _techCompanyRepo,
            IRepository<DeliveryPerson> _deliveryPersonRepo,
            IRepository<Cart> _cartRepo,
            IRepository<WishList> _wishListRepo,
            TechpertsContext _context,
            ICustomerService _customerService)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            adminRepo = _adminRepo;
            customerRepo = _customerRepo;
            techCompanyRepo = _techCompanyRepo;
            deliveryPersonRepo = _deliveryPersonRepo;
            cartRepo = _cartRepo;
            wishListRepo = _wishListRepo;
            context = _context;
            customerService = _customerService;
        }
        [HttpPost("check-role")]
        public async Task<IActionResult> CheckRole([FromBody] string roleName)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);

            return Ok(new GeneralResponse<bool>
            {
                Success = true,
                Message = exists ? "Role exists." : "Role does not exist.",
                Data = exists
            });
        }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] RoleDTO dto, RoleType roleName)
        {
            var user = await userManager.FindByEmailAsync(dto.userEmail);

            if (user == null)
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = $"{dto.userEmail} is not registered"
                });
            var result = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to assign role",
                    Data = string.Join(" ", result.Errors.Select(e => e.Description))
                });
            }
            var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
            if (role == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Role '{roleName}' does not exist.",
                    Data = $"{roleName} not found"
                });
            }
            switch (roleName)
            {
                case RoleType.Customer:
                    if (!await customerRepo.AnyAsync(c => c.UserId == user.Id))
                    {
                        var newCustomer = new Customer
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        };

                        await customerRepo.AddAsync(newCustomer);
                        await context.SaveChangesAsync(); 

                        var newCart = new Cart
                        {
                            CustomerId = newCustomer.Id,
                            CreatedAt = DateTime.UtcNow,
                            CartItems = new List<CartItem>() 
                        };

                        await cartRepo.AddAsync(newCart);

                        var newWishlist = new WishList 
                        {
                            CustomerId = newCustomer.Id,
                            CreatedAt = DateTime.UtcNow,
                            WishListItems = new List<WishListItem>()
                        };
                        await wishListRepo.AddAsync(newWishlist);

                        await context.SaveChangesAsync();
                    }
                    break;

                case RoleType.Admin:
                    if (!await adminRepo.AnyAsync(a => a.UserId == user.Id))
                    {
                        await adminRepo.AddAsync(new Admin
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case RoleType.TechCompany:
                    if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == user.Id))
                    {
                        await techCompanyRepo.AddAsync(new TechCompany
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case RoleType.DeliveryPerson:
                    if (!await deliveryPersonRepo.AnyAsync(dp => dp.UserId == user.Id))
                    {
                        await deliveryPersonRepo.AddAsync(new DeliveryPerson
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;
            }
            await context.SaveChangesAsync();

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = $"Role '{roleName}' assigned to user '{dto.userEmail}' successfully.",
                Data = $"Role '{roleName}' assigned to user '{dto.userEmail}' successfully."
            });
        }

        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignRole([FromBody] RoleDTO dto, RoleType roleName)
        {
            var user = await userManager.FindByEmailAsync(dto.userEmail);
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = dto.userEmail
                });
            }

            if (!await userManager.IsInRoleAsync(user, roleName.GetStringValue()))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"User is not assigned to role '{roleName}'.",
                    Data = $"{dto.userEmail} is not assigned as {roleName}"
                });
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to remove role '{roleName}' from user.",
                    Data = string.Join(" ", result.Errors.Select(e => e.Description))
                });
            }

            switch (roleName)
            {
                case RoleType.Customer:
                    await customerService.CleanupCustomerDataAsync(user.Id); 
                    break;

                case RoleType.Admin:
                    var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(a => a.UserId == user.Id);
                    if (admin != null) adminRepo.Remove(admin);
                    break;

                case RoleType.TechCompany:
                    var techCompany = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(tc => tc.UserId == user.Id);
                    if (techCompany != null) techCompanyRepo.Remove(techCompany);
                    break;

                case RoleType.DeliveryPerson:
                    var deliveryPerson = (await deliveryPersonRepo.GetAllAsync()).FirstOrDefault(dp => dp.UserId == user.Id);
                    if (deliveryPerson != null) deliveryPersonRepo.Remove(deliveryPerson);
                    break;
            }

            await context.SaveChangesAsync();

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = $"Role '{roleName}' unassigned and domain entity removed for user '{dto.userEmail}'.",
                Data = $"Role {roleName} no longer assigned to {dto.userEmail}"
            });
        }
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            if (roles.Count == 0)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No roles found.",
                    Data = "no roles set yet"
                });
            }
            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = "Roles retrieved successfully.",
                Data = roles
            });
        }

        [HttpGet("registration-options")]
        public IActionResult GetRegistrationRoleOptions()
        {
            var roleOptions = Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(role => new RoleOptionDTO
                {
                    Value = role.ToString(),
                    DisplayName = role.GetStringValue(),
                    Description = GetRoleDescription(role)
                })
                .ToList();

            return Ok(new GeneralResponse<List<RoleOptionDTO>>
            {
                Success = true,
                Message = "Registration role options retrieved successfully.",
                Data = roleOptions
            });
        }

        [HttpGet("enum-values")]
        public IActionResult GetRoleEnumValues()
        {
            var roleValues = Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(role => new
                {
                    Value = role.ToString(),
                    DisplayName = role.GetStringValue()
                })
                .ToList();

            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = "Role enum values retrieved successfully.",
                Data = roleValues
            });
        }

        private string GetRoleDescription(RoleType role)
        {
            return role switch
            {
                RoleType.Customer => "Can browse products, manage cart, place orders, and manage wishlist",
                RoleType.Admin => "Full system access, user management, and system configuration",
                RoleType.TechCompany => "Product management, maintenance services, and warranties",
                RoleType.DeliveryPerson => "Order delivery management and status updates",
                _ => "Unknown role"
            };
        }
    }
}
