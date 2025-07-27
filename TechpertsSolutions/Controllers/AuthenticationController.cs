using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;       
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.LoginDTOs;
using TechpertsSolutions.Core.DTOs.RegisterDTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using TechpertsSolutions.Utilities;

namespace TechpertsSolutions.Controllers
{

    //public class AuthenticationController : ControllerBase
    //{
    //    private readonly IRepository<Admin> adminRepo;
    //    private readonly IRepository<Customer> customerRepo;
    //    private readonly IRepository<SalesManager> salesManagerRepo;
    //    private readonly IRepository<StockControlManager> stockControlManagerRepo;
    //    private readonly IRepository<TechCompany> techCompanyRepo;
    //    private readonly IRepository<TechManager> techManagerRepo;
    //    private readonly UserManager<AppUser> userManager;
    //    private readonly RoleManager<AppRole> roleManager;
    //    private readonly IRepository<Cart> cartRepo;
    //    private readonly IConfiguration configuration; 
    //    private readonly TechpertsContext context;
    //    private readonly ICustomerService customerService;

    //    public AuthenticationController(UserManager<AppUser> _userManager,
    //        RoleManager<AppRole> _roleManager,
    //        IRepository<Cart> _cartRepo,
    //        IRepository<Admin> _adminRepo,
    //        IRepository<Customer> _customerRepo,
    //        IRepository<SalesManager> _salesManagerRepo,
    //        IRepository<StockControlManager> _stockControlMangerRepo,
    //        IRepository<TechCompany> _techCompanyRepo,
    //        IRepository<TechManager> _techMangerRepo,
    //        TechpertsContext _context,
    //        IConfiguration _configuration,
    //        ICustomerService _customerService)
    //    {
    //        userManager = _userManager;
    //        roleManager = _roleManager;
    //        cartRepo = _cartRepo;
    //        adminRepo = _adminRepo;
    //        customerRepo = _customerRepo;
    //        salesManagerRepo = _salesManagerRepo;
    //        stockControlManagerRepo = _stockControlMangerRepo;
    //        techCompanyRepo = _techCompanyRepo;
    //        techManagerRepo = _techMangerRepo;
    //        context = _context;
    //        configuration = _configuration;
    //        customerService = _customerService;
    //    }

    //    [HttpPost("register")]
    //    public async Task<IActionResult> Register([FromForm] RegisterDTO register)
    //    {
    //        var user = new AppUser
    //        {
    //            FullName = register.FullName,
    //            UserName = register.UserName,
    //            Email = register.Email,
    //            Address = register.Address,
    //            PhoneNumber = register.PhoneNumber
    //        };
    //        var result = await userManager.CreateAsync(user, register.Password);
    //        if (result.Succeeded)
    //        {
    //            return Ok(new GeneralResponse<string>
    //            {
    //                Success = true,
    //                Message = "User registered successfully.",
    //                Data = user.Id
    //            });
    //        }
    //        else
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "User registration failed.",
    //                Data = string.Join(", ", result.Errors.Select(e => e.Description))
    //            });
    //        }
    //    }
    //    [HttpPost("login")]
    //    public async Task<IActionResult> Login([FromForm] LoginDTO login)
    //    {

    //        var user = await userManager.FindByEmailAsync(login.Email);
    //        if (user != null)
    //        {
    //            var result = await userManager.CheckPasswordAsync(user, login.Password);
    //            if (result)
    //            {
    //                var token = GenerateJwtToken(user);
    //                var roleName = await userManager.GetRolesAsync(user);
    //                return Ok(new GeneralResponse<LoginResultDTO>
    //                {
    //                    Success = true,
    //                    Message = "User logged in successfully.",
    //                    Data = new LoginResultDTO
    //                    {
    //                        Token = token,
    //                        UserId = user.Id,
    //                        UserName = user.UserName,
    //                        RoleName = roleName
    //                    }
    //                });
    //            }
    //        }
    //        return BadRequest(new GeneralResponse<string>
    //        {
    //            Success = false,
    //            Message = "Invalid login attempt.",
    //            Data = $"Login email {login.Email} is invalid or password incorrect."
    //        });
    //    }
    //    [HttpPost("forgot-password")]
    //    public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPassword) 
    //    {
    //        var user = await userManager.FindByEmailAsync(forgotPassword.Email);
    //        if (user == null) 
    //        {
    //            return NotFound(new GeneralResponse<string> 
    //            {
    //                Success = false,
    //                Message = "User not found",
    //                Data = $"user email {forgotPassword.Email.ToString()} not registered"
    //            });
    //        }
    //        var token = await userManager.GeneratePasswordResetTokenAsync(user);
    //        return Ok(new GeneralResponse<string> 
    //        {
    //            Success = true,
    //            Message = "Password reset token generated successfully.",
    //            Data = token
    //        });
    //    }
    //    [HttpPost("reset-password")]
    //    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPassword) 
    //    {
    //        var user = await userManager.FindByEmailAsync(resetPassword.Email);
    //        if (user == null) 
    //        {
    //            return NotFound(new GeneralResponse<string> 
    //            {
    //                Success = false,
    //                Message = "User not found",
    //                Data = $"user email {resetPassword.Email.ToString()} not registered"
    //            });
    //        }
    //        var result = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
    //        if (!result.Succeeded) 
    //        {
    //            return BadRequest(new GeneralResponse<string> 
    //            {
    //                Success = false,
    //                Message = "Password reset failed.",
    //                Data = string.Join(", ", result.Errors.Select(e => e.Description))
    //            });

    //        }
    //        return Ok(new GeneralResponse<string> 
    //        {
    //            Success = true,
    //            Message = "Password reset successfully.",
    //            Data = user.Id
    //        });
    //    }

    //    [HttpDelete("delete-account")]
    //    [Authorize]
    //    public async Task<IActionResult> DeleteAccount([FromForm] DeleteAccountDTO request)
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        if (string.IsNullOrEmpty(userId))
    //        {
    //            return Unauthorized(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Invalid token or user context.",
    //                Data = null
    //            });
    //        }

    //        var user = await userManager.FindByIdAsync(userId);
    //        if (user == null || user.Email != request.Email)
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Email does not match the authenticated user.",
    //                Data = request.Email
    //            });
    //        }

    //        var roles = await userManager.GetRolesAsync(user);
    //        foreach (var roleName in roles)
    //        {
    //            switch (roleName)
    //            {
    //                case "Customer":
    //                    await customerService.CleanupCustomerDataAsync(user.Id);
    //                    break;

    //                case "SaleManager":
    //                    var salesManager = (await salesManagerRepo.GetAllAsync()).FirstOrDefault(sm => sm.UserId == user.Id);
    //                    if (salesManager != null) salesManagerRepo.Remove(salesManager);
    //                    break;

    //                case "TechManager":
    //                    var techManager = (await techManagerRepo.GetAllAsync()).FirstOrDefault(tm => tm.UserId == user.Id);
    //                    if (techManager != null) techManagerRepo.Remove(techManager);
    //                    break;

    //                case "StockControlManager":
    //                    var stockManager = (await stockControlManagerRepo.GetAllAsync()).FirstOrDefault(sc => sc.UserId == user.Id);
    //                    if (stockManager != null) stockControlManagerRepo.Remove(stockManager);
    //                    break;

    //                case "Admin":
    //                    var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(a => a.UserId == user.Id);
    //                    if (admin != null) adminRepo.Remove(admin);
    //                    break;

    //                case "TechCompany":
    //                    var techCompany = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(tc => tc.UserId == user.Id);
    //                    if (techCompany != null) techCompanyRepo.Remove(techCompany);
    //                    break;
    //            }

    //            await userManager.RemoveFromRoleAsync(user, roleName);
    //            await context.SaveChangesAsync();
    //        }

    //        var deleteResult = await userManager.DeleteAsync(user);
    //        if (!deleteResult.Succeeded)
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Failed to delete user account.",
    //                Data = string.Join(", ", deleteResult.Errors.Select(e => e.Description))
    //            });
    //        }

    //        return Ok(new GeneralResponse<string>
    //        {
    //            Success = true,
    //            Message = "Your account has been deleted successfully.",
    //            Data = user.Email
    //        });
    //    }
    //    private string GenerateJwtToken(AppUser user)
    //    {
    //        var claims = new[]
    //         {
    //            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //            new Claim(ClaimTypes.NameIdentifier, user.Id)
    //        };

    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //        var token = new JwtSecurityToken(
    //            issuer: configuration["Jwt:Issuer"],
    //            audience: configuration["Jwt:Audience"],
    //            claims: claims,
    //            expires: DateTime.Now.AddMinutes(30),
    //            signingCredentials: creds);

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }
    //}
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto, RoleType role)
        {
            // Check if model state is valid (validation attributes)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = string.Join("; ", errors)
                });
            }

            var response = await _authService.RegisterAsync(dto,role);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // Check if model state is valid (validation attributes)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = string.Join("; ", errors)
                });
            }

            var response = await _authService.LoginAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            // Check if model state is valid (validation attributes)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = string.Join("; ", errors)
                });
            }

            var response = await _authService.ForgotPasswordAsync(dto);
            
            // For development/testing purposes, if email is not configured, return the token
            if (response.Success && response.Data != null)
            {
                // Check if email configuration is empty (development mode)
                var smtpServer = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Email:SmtpServer"];
                if (string.IsNullOrEmpty(smtpServer))
                {
                    // Return token for testing purposes
                    return Ok(new GeneralResponse<string>
                    {
                        Success = true,
                        Message = "Password reset token generated successfully. (Development mode - email not configured)",
                        Data = response.Data // This contains the token
                    });
                }
            }
            
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            // Check if model state is valid (validation attributes)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = string.Join("; ", errors)
                });
            }

            var response = await _authService.ResetPasswordAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO dto)
        {
            // Check if model state is valid (validation attributes)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = string.Join("; ", errors)
                });
            }

            // Get the user ID and email from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid token or user context.",
                    Data = null
                });
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User email not found in token.",
                    Data = null
                });
            }

            // For security, we don't need the email in the request body since we get it from the token
            // But we can validate that the user is aware of their email
            Console.WriteLine($"Delete Account Request:");
            Console.WriteLine($"User ID from token: {userId}");
            Console.WriteLine($"User Email from token: {userEmail}");
            Console.WriteLine($"Password provided: {!string.IsNullOrEmpty(dto.Password)}");

            var response = await _authService.DeleteAccountAsync(dto, userId);
            return StatusCode(response.Success ? 200 : 400, response);
        }
    }
}
