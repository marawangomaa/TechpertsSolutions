using Core.Enums;
using Core.Interfaces;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;


namespace TechpertsSolutions.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRepository<Admin> adminRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<SalesManager> salesManagerRepo;
        private readonly IRepository<StockControlManager> stockControlManagerRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<TechManager> techManagerRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly TechpertsContext context;
        private readonly ICustomerService customerService;

        public RolesController(RoleManager<AppRole> _roleManager, 
            UserManager<AppUser> _userManager,
            IRepository<Admin> _adminRepo,
            IRepository<Customer> _customerRepo,
            IRepository<SalesManager> _salesManagerRepo,
            IRepository<StockControlManager> _stockControlMangerRepo,
            IRepository<TechCompany> _techCompanyRepo,
            IRepository<TechManager> _techMangerRepo,
            IRepository<Cart> _cartRepo,
            TechpertsContext _context,
            ICustomerService _customerService) 
        {
            roleManager = _roleManager;
            userManager = _userManager;
            adminRepo = _adminRepo;
            customerRepo = _customerRepo;
            salesManagerRepo = _salesManagerRepo;
            stockControlManagerRepo = _stockControlMangerRepo;
            techCompanyRepo = _techCompanyRepo;
            techManagerRepo = _techMangerRepo;
            cartRepo = _cartRepo;
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
        //[HttpPost("assign")]
        //public async Task<IActionResult> AssignRole(string userEmail, RoleType roleName) 
        //{
        //    var user = await userManager.FindByEmailAsync(userEmail);

        //    if (user == null)
        //        return NotFound(new GeneralResponse<string> 
        //        {
        //            Success = false,
        //            Message = "User not found.",
        //            Data = $"{userEmail} is not registered"
        //        });
        //    var result = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
        //    if (!result.Succeeded) 
        //    {
        //        return BadRequest(new GeneralResponse<string> 
        //        {
        //            Success = false,
        //            Message = "Failed to assign role",
        //            Data = result.Errors.Select(e => e.Description)
        //        });
        //    }
        //    var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
        //    if (role.Id == null)
        //    {
        //        return BadRequest(new GeneralResponse<string>
        //        {
        //            Success = false,
        //            Message = $"Role '{roleName}' does not exist.",
        //            Data = $"{roleName} not found"
        //        });
        //    }
        //    switch (roleName)
        //    {
        //        case RoleType.SaleManager:
        //            if (!await salesManagerRepo.AnyAsync(sm => sm.UserId == user.Id))
        //            {
        //                 await salesManagerRepo.AddAsync(new SalesManager 
        //                { 
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                });
        //            }
        //            break;

        //        case RoleType.Customer:
        //            if (!await customerRepo.AnyAsync(c => c.UserId == user.Id))
        //            {
        //                var newCustomer = new Customer
        //                {
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                };

        //                await customerRepo.AddAsync(newCustomer);
        //                await context.SaveChangesAsync();

        //                var newCart = new Cart
        //                {
        //                    CustomerId = newCustomer.Id,
        //                    CreatedAt = DateTime.UtcNow,
        //                    CartItems = new List<CartItem>()
        //                };

        //                await cartRepo.AddAsync(newCart);

        //            }
        //            break;

        //        case RoleType.TechManager:
        //            if (!await techManagerRepo.AnyAsync(t => t.UserId == user.Id))
        //            {
        //                await techManagerRepo.AddAsync(new TechManager 
        //                { 
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                });
        //            }
        //            break;

        //        case RoleType.StockControlManager:
        //            if (!await stockControlManagerRepo.AnyAsync(s => s.UserId == user.Id))
        //            {
        //                await stockControlManagerRepo.AddAsync(new StockControlManager 
        //                { 
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                });
        //            }
        //            break;

        //        case RoleType.Admin:
        //            if (!await adminRepo.AnyAsync(a => a.UserId == user.Id))
        //            {
        //                await adminRepo.AddAsync(new Admin 
        //                { 
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                });
        //            }
        //            break;

        //        case RoleType.TechCompany:
        //            if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == user.Id))
        //            {
        //                await  techCompanyRepo.AddAsync(new TechCompany 
        //                { 
        //                    UserId = user.Id,
        //                    RoleId = role.Id
        //                });
        //            }
        //            break;
        //    }
        //    await context.SaveChangesAsync();

        //    return Ok(new GeneralResponse<string> 
        //    {
        //        Success = true,
        //        Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
        //        Data = $"Role '{roleName}' assigned to user '{userEmail}' successfully."
        //    });
        //}

        //[HttpPost("unassign")]
        //public async Task<IActionResult> UnassignRole(string userEmail, RoleType roleName)
        //{
        //    var user = await userManager.FindByEmailAsync(userEmail);
        //    if (user == null)
        //    {
        //        return NotFound(new GeneralResponse<string>
        //        {
        //            Success = false,
        //            Message = "User not found.",
        //            Data = userEmail
        //        });
        //    }

        //    if (!await userManager.IsInRoleAsync(user, roleName.GetStringValue()))
        //    {
        //        return BadRequest(new GeneralResponse<string>
        //        {
        //            Success = false,
        //            Message = $"User is not assigned to role '{roleName}'.",
        //            Data = $"{userEmail} is not assigned as {roleName}"
        //        });
        //    }

        //    var result = await userManager.RemoveFromRoleAsync(user, roleName.GetStringValue());
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(new GeneralResponse<string>
        //        {
        //            Success = false,
        //            Message = $"Failed to remove role '{roleName}' from user.",
        //            Data = result.Errors.Select(e => e.Description)
        //        });
        //    }

        //    switch (roleName)
        //    {
        //        case RoleType.Customer:
        //            await customerService.CleanupCustomerDataAsync(user.Id);
        //            break;

        //        case RoleType.SaleManager:
        //            var salesManager = (await salesManagerRepo.GetAllAsync()).FirstOrDefault(sm => sm.UserId == user.Id);
        //            if (salesManager != null) salesManagerRepo.Remove(salesManager);
        //            break;

        //        case RoleType.TechManager:
        //            var techManager = (await techManagerRepo.GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id);
        //            if (techManager != null) techManagerRepo.Remove(techManager);
        //            break;

        //        case RoleType.StockControlManager:
        //            var stockControlManager = (await stockControlManagerRepo.GetAllAsync()).FirstOrDefault(s => s.UserId == user.Id);
        //            if (stockControlManager != null) stockControlManagerRepo.Remove(stockControlManager);
        //            break;

        //        case RoleType.Admin:
        //            var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(a => a.UserId == user.Id);
        //            if (admin != null) adminRepo.Remove(admin);
        //            break;

        //        case RoleType.TechCompany:
        //            var techCompany = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(tc => tc.UserId == user.Id);
        //            if (techCompany != null) techCompanyRepo.Remove(techCompany);
        //            break;
        //    }

        //    await context.SaveChangesAsync();

        //    return Ok(new GeneralResponse<string>
        //    {
        //        Success = true,
        //        Message = $"Role '{roleName}' unassigned and domain entity removed for user '{userEmail}'.",
        //        Data = $"Role {roleName} no longer assigned to {userEmail}"
        //    });
        //}

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(string userEmail, RoleType roleName)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || !new EmailAddressAttribute().IsValid(userEmail))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid email format.",
                    Data = userEmail
                });
            }

            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = $"{userEmail} is not registered"
                });

            var result = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to assign role",
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
            if (role?.Id == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Role '{roleName}' does not exist.",
                    Data = $"{roleName} not found"
                });
            }

            await context.SaveChangesAsync();

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                Data = $"Role '{roleName}' assigned to user '{userEmail}' successfully."
            });
        }

        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignRole(string userEmail, RoleType roleName)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || !new EmailAddressAttribute().IsValid(userEmail))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid email format.",
                    Data = userEmail
                });
            }

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = userEmail
                });
            }

            if (!await userManager.IsInRoleAsync(user, roleName.GetStringValue()))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"User is not assigned to role '{roleName}'.",
                    Data = $"{userEmail} is not assigned as {roleName}"
                });
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to remove role '{roleName}' from user.",
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            await context.SaveChangesAsync();

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = $"Role '{roleName}' unassigned and domain entity removed for user '{userEmail}'.",
                Data = $"Role {roleName} no longer assigned to {userEmail}"
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
            return Ok(new GeneralResponse<string> 
            {
                Success = true,
                Message = "Roles retrieved successfully.",
                Data = roles
            });
        }
    }
}
