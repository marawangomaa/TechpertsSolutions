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
            
            
            if (response.Success && response.Data != null)
            {
                
                var smtpServer = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Email:SmtpServer"];
                if (string.IsNullOrEmpty(smtpServer))
                {
                    
                    return Ok(new GeneralResponse<string>
                    {
                        Success = true,
                        Message = "Password reset token generated successfully. (Development mode - email not configured)",
                        Data = response.Data 
                    });
                }
            }
            
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            
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

            
            
            Console.WriteLine($"Delete Account Request:");
            Console.WriteLine($"User ID from token: {userId}");
            Console.WriteLine($"User Email from token: {userEmail}");
            Console.WriteLine($"Password provided: {!string.IsNullOrEmpty(dto.Password)}");

            var response = await _authService.DeleteAccountAsync(dto, userId);
            return StatusCode(response.Success ? 200 : 400, response);
        }
    }
}
