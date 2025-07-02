using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;

        public AuthenticationController(UserManager<AppUser> _userManager, IConfiguration _configuration)
        {
            userManager = _userManager;
            configuration = _configuration;
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
                    var token = GenerateJwtToken(user);
                    return Ok(new GeneralResponse<string>
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
                Data = null
            });
        }
        private string GenerateJwtToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
