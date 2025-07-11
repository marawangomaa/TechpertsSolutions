using Core.DTOs.Login;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;       
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.Login;
using TechpertsSolutions.Core.DTOs.Register;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using TechpertsSolutions.Utilities;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly TechpertsContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;

        public AuthenticationController(UserManager<AppUser> _userManager,RoleManager<AppRole> _roleManager, TechpertsContext _context,ITokenService _tokenService)
        {
            userManager = _userManager;
            tokenService = _tokenService;
            context = _context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            var user = new AppUser
            {
                FullName = register.FullName,
                UserName = register.UserName,
                Email = register.Email,
                Address = register.Address,
                PhoneNumber = register.PhoneNumber
            };
            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                return Ok(new GeneralResponse<string>
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = user.Id
                });
            }
            else
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User registration failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {

            var user = await userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, login.Password);
                if (result)
                {
                    var token = tokenService.GenerateJwtToken(user);
                    return Ok(new GeneralResponse<LoginResultDTO>
                    {
                        Success = true,
                        Message = "User logged in successfully.",
                        Data = new LoginResultDTO
                        {
                            Token = token,
                            UserId = user.Id,
                            UserName = user.UserName
                        }
                    });
                }
            }
            return BadRequest(new GeneralResponse<string>
            {
                Success = false,
                Message = "Invalid login attempt.",
                Data = $"Login email {login.Email} is invalid or password incorrect."
            });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPassword) 
        {
            var user = await userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null) 
            {
                return NotFound(new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "User not found",
                    Data = $"user email {forgotPassword.Email.ToString()} not registered"
                });
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            // ⚠️ In real apps: send this token as a secure email link
            return Ok(new GeneralResponse<string> 
            {
                Success = true,
                Message = "Password reset token generated successfully.",
                Data = token //In real apps, send this token via email : return URL instead for production
            });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword) 
        {
            var user = await userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null) 
            {
                return NotFound(new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "User not found",
                    Data = $"user email {resetPassword.Email.ToString()} not registered"
                });
            }
            var result = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (!result.Succeeded) 
            {
                return BadRequest(new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "Password reset failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                });

            }
            return Ok(new GeneralResponse<string> 
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = user.Id
            });
        }
        // var resetUrl = $"https://yourfrontend.com/reset-password?email={user.Email}&token={HttpUtility.UrlE



        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // From JWT
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid token or user context.",
                    Data = null
                });
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.Email != request.Email)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Email does not match the authenticated user.",
                    Data = request.Email
                });
            }

            // Unassign roles and remove domain-specific entities
            var roles = await userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                switch (roleName)
                {
                    case "SalesManager":
                        var salesManager = await context.SalesManagers.FirstOrDefaultAsync(sm => sm.UserId == user.Id);
                        if (salesManager != null) context.SalesManagers.Remove(salesManager);
                        break;

                    case "Customer":
                        var customer = await context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                        if (customer != null) context.Customers.Remove(customer);
                        break;

                    case "TechManager":
                        var techManager = await context.TechManagers.FirstOrDefaultAsync(tm => tm.UserId == user.Id);
                        if (techManager != null) context.TechManagers.Remove(techManager);
                        break;

                    case "StockControlManager":
                        var stockManager = await context.StockControlManagers.FirstOrDefaultAsync(sc => sc.UserId == user.Id);
                        if (stockManager != null) context.StockControlManagers.Remove(stockManager);
                        break;

                    case "Admin":
                        var admin = await context.Admins.FirstOrDefaultAsync(a => a.UserId == user.Id);
                        if (admin != null) context.Admins.Remove(admin);
                        break;

                    case "TechCompany":
                        var techCompany = await context.TechCompanies.FirstOrDefaultAsync(tc => tc.UserId == user.Id);
                        if (techCompany != null) context.TechCompanies.Remove(techCompany);
                        break;
                }

                // After DB changes are set up
                await context.SaveChangesAsync();
                await userManager.RemoveFromRoleAsync(user, roleName);
            }

            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to delete user account.",
                    Data = string.Join(", ", deleteResult.Errors.Select(e => e.Description))
                });
            }

            await context.SaveChangesAsync();

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Your account has been deleted successfully.",
                Data = user.Email
            });
        }
    }
}
