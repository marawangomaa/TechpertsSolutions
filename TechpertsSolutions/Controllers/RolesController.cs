using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly TechpertsContext context;

        public RolesController(RoleManager<AppRole> _roleManager, 
            UserManager<AppUser> _userManager,
            IRepository<Admin> _adminRepo,
            IRepository<Customer> _customerRepo,
            IRepository<SalesManager> _salesManagerRepo,
            IRepository<StockControlManager> _stockControlMangerRepo,
            IRepository<TechCompany> _techCompanyRepo,
            IRepository<TechManager> _techMangerRepo,
            TechpertsContext _context) 
        {
            roleManager = _roleManager;
            userManager = _userManager;
            adminRepo = _adminRepo;
            customerRepo = _customerRepo;
            salesManagerRepo = _salesManagerRepo;
            stockControlManagerRepo = _stockControlMangerRepo;
            techCompanyRepo = _techCompanyRepo;
            techManagerRepo = _techMangerRepo;
            context = _context;
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
        public async Task<IActionResult> AssignRole(string userEmail, string roleName) 
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "User not found.",
                    Data = $"{userEmail} is not registered"
                });
            var result = await userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded) 
            {
                return BadRequest(new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "Failed to assign role",
                    Data = result.Errors.Select(e => e.Description)
                });
            }
            var role = await roleManager.FindByNameAsync(roleName);
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
                case "SalesManager":
                    if (!await salesManagerRepo.AnyAsync(sm => sm.UserId == user.Id))
                    {
                         await salesManagerRepo.AddAsync(new SalesManager 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "Customer":
                    if (!await customerRepo.AnyAsync(c => c.UserId == user.Id))
                    {
                         await customerRepo.AddAsync(new Customer 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "TechManager":
                    if (!await techManagerRepo.AnyAsync(t => t.UserId == user.Id))
                    {
                        await techManagerRepo.AddAsync(new TechManager 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "StockControlManager":
                    if (!await stockControlManagerRepo.AnyAsync(s => s.UserId == user.Id))
                    {
                        await stockControlManagerRepo.AddAsync(new StockControlManager 
                        { 
                            UserId = user.Id,
                            RoleId = user.Id
                        });
                    }
                    break;

                case "Admin":
                    if (!await adminRepo.AnyAsync(a => a.UserId == user.Id))
                    {
                        await adminRepo.AddAsync(new Admin 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "TechCompany":
                    if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == user.Id))
                    {
                        await  techCompanyRepo.AddAsync(new TechCompany 
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
                Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                Data = $"Role '{roleName}' assigned to user '{userEmail}' successfully."
            });
        }

        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignRole(string userEmail, string roleName)
        {
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

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"User is not assigned to role '{roleName}'.",
                    Data = $"{userEmail} is not assigned as {roleName}"
                });
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to remove role '{roleName}' from user.",
                    Data = result.Errors.Select(e => e.Description)
                });
            }

            // 🧼 Clean up from domain table if applicable
            switch (roleName)
            {
                case "SalesManager":
                    var salesManager = (await salesManagerRepo.GetAllAsync()).FirstOrDefault(sm => sm.UserId == user.Id);
                    if (salesManager != null)
                        salesManagerRepo.Remove(salesManager);
                    break;

                case "Customer":
                    var customer = (await customerRepo.GetAllAsync()).FirstOrDefault(c => c.UserId == user.Id);
                    if (customer != null)
                        customerRepo.Remove(customer);
                    break;

                case "TechManager":
                    var techManager = (await techManagerRepo.GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id);
                    if (techManager != null)
                        techManagerRepo.Remove(techManager);
                    break;
                case "StockControlManager":
                    var stockControlManager = (await stockControlManagerRepo.GetAllAsync()).FirstOrDefault(s => s.UserId == user.Id);
                    if (stockControlManager != null)
                        stockControlManagerRepo.Remove(stockControlManager);
                    break;
                case "Admin":
                    var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(a => a.UserId == user.Id);
                    if (admin != null)
                        adminRepo.Remove(admin);
                    break;
                case "TechCompany":
                    var techCompany = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(tc => tc.UserId == user.Id);
                    if (techCompany != null)
                        techCompanyRepo.Remove(techCompany);
                    break;

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
