using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.DTOs;
using TechpertsSolutions.Repository.Data;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly TechpertsContext context;
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RolesController(RoleManager<AppRole> _roleManager, UserManager<AppUser> _userManager,TechpertsContext _context) 
        {
            context = _context;
            roleManager = _roleManager;
            userManager = _userManager;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName) 
        {
            if (await roleManager.RoleExistsAsync(roleName)) 
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Role already exists.",
                    Data = roleName
                });
            }
            await roleManager.CreateAsync(new AppRole { Name = roleName });
            
            return Ok(new GeneralResponse<string> 
            {
                Success = true,
                Message = "Role created successfully.",
                Data = roleName
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
                    if (!context.SalesManagers.Any(sm => sm.UserId == user.Id))
                    {
                        context.SalesManagers.Add(new SalesManager 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "Customer":
                    if (!context.Customers.Any(c => c.UserId == user.Id))
                    {
                        context.Customers.Add(new Customer 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "TechManager":
                    if (!context.TechManagers.Any(t => t.UserId == user.Id))
                    {
                        context.TechManagers.Add(new TechManager 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "StockControlManager":
                    if (!context.StockControlManagers.Any(s => s.UserId == user.Id))
                    {
                        context.StockControlManagers.Add(new StockControlManager 
                        { 
                            UserId = user.Id,
                            RoleId = user.Id
                        });
                    }
                    break;

                case "Admin":
                    if (!context.Admins.Any(a => a.UserId == user.Id))
                    {
                        context.Admins.Add(new Admin 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;

                case "TechCompany":
                    if (!context.TechCompanies.Any(tc => tc.UserId == user.Id))
                    {
                        context.TechCompanies.Add(new TechCompany 
                        { 
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    break;
            }

            // 💾 Commit domain updates
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
                    var salesManager = context.SalesManagers.FirstOrDefault(sm => sm.UserId == user.Id);
                    if (salesManager != null)
                        context.SalesManagers.Remove(salesManager);
                    break;

                case "Customer":
                    var customer = context.Customers.FirstOrDefault(c => c.UserId == user.Id);
                    if (customer != null)
                        context.Customers.Remove(customer);
                    break;

                case "TechManager":
                    var techManager = context.TechManagers.FirstOrDefault(t => t.UserId == user.Id);
                    if (techManager != null)
                        context.TechManagers.Remove(techManager);
                    break;
                case "StockControlManager":
                    var stockControlManager = context.StockControlManagers.FirstOrDefault(s => s.UserId == user.Id);
                    if (stockControlManager != null)
                        context.StockControlManagers.Remove(stockControlManager);
                    break;
                case "Admin":
                    var admin = context.Admins.FirstOrDefault(a => a.UserId == user.Id);
                    if (admin != null)
                        context.Admins.Remove(admin);
                    break;
                case "TechCompany":
                    var techCompany = context.TechCompanies.FirstOrDefault(tc => tc.UserId == user.Id);
                    if (techCompany != null)
                        context.TechCompanies.Remove(techCompany);
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
