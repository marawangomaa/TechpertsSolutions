using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using TechpertsSolutions.Core.DTOs.LoginDTOs;
using TechpertsSolutions.Core.DTOs.RegisterDTOs;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly IWebHostEnvironment _env;

        public AuthenticationController(
            IAuthService authService,
            IEmailService emailService,
            IConfiguration configuration,
            UserManager<AppUser> _userManager,
            IWebHostEnvironment env)
        {
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
            userManager = _userManager;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO dto, [FromForm] RoleType role)
        {
            if (!ModelState.IsValid) return ValidationError();

            var response = await _authService.RegisterAsync(dto, role);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO dto)
        {
            if (!ModelState.IsValid) return ValidationError();

            var response = await _authService.LoginAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid) return ValidationError();
            var response = await _authService.ForgotPasswordAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid) return ValidationError();
            var response = await _authService.ResetPasswordAsync(dto);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromForm] DeleteAccountDTO dto)
        {
            if (!ModelState.IsValid) return ValidationError();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid token or user context.",
                    Data = null
                });
            }

            var response = await _authService.DeleteAccountAsync(dto, userId);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail([FromQuery] string to)
        {
            try
            {
                var body = "<h1>Hello from Techperts Solutions!</h1><p>This is a test email.</p>";
                var success = await _emailService.SendEmailAsync(to, "Test Email", body);

                if (success)
                    return Ok(new { Success = true, Message = "Email sent successfully!" });

                return StatusCode(500, new { Success = false, Message = "Failed to send email." });
            }
            catch (Exception ex)
            {
                // Return the exact SMTP/Gmail error
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Email sending failed",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }



        private IActionResult ValidationError()
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
    }
}