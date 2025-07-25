using Azure.Core;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Core.DTOs.LoginDTOs;
using TechpertsSolutions.Core.DTOs.RegisterDTOs;
using TechpertsSolutions.Repository.Data;
using Core.DTOs.LoginDTOs;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IRepository<Admin> adminRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<SalesManager> salesManagerRepo;
        private readonly IRepository<StockControlManager> stockControlManagerRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<TechManager> techManagerRepo;
        private readonly ICustomerService customerService;
        private readonly IWishListService wishListService;
        private readonly IConfiguration configuration;
        private readonly TechpertsContext context;

        public AuthService(UserManager<AppUser> _userManager,
            RoleManager<AppRole> _roleManager,
            IRepository<Cart> _cartRepo,
            IRepository<Admin> _adminRepo,
            IRepository<Customer> _customerRepo,
            IRepository<SalesManager> _salesManagerRepo,
            IRepository<StockControlManager> _stockControlMangerRepo,
            IRepository<TechCompany> _techCompanyRepo,
            IRepository<TechManager> _techMangerRepo,
            TechpertsContext _context,
            IConfiguration _configuration,
            ICustomerService _customerService,
            IWishListService _wishListService)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            cartRepo = _cartRepo;
            adminRepo = _adminRepo;
            customerRepo = _customerRepo;
            salesManagerRepo = _salesManagerRepo;
            stockControlManagerRepo = _stockControlMangerRepo;
            techCompanyRepo = _techCompanyRepo;
            techManagerRepo = _techMangerRepo;
            context = _context;
            configuration = _configuration;
            customerService = _customerService;
            wishListService = _wishListService;
        }

        public async Task<GeneralResponse<string>> DeleteAccountAsync(DeleteAccountDTO dto, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid token or user context.",
                    Data = null
                };
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.Email != dto.Email)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Email does not match the authenticated user.",
                    Data = null
                };
            }

            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User has no assigned roles.",
                    Data = null
                };
            }

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                foreach (var roleName in roles)
                {
                    switch (roleName)
                    {
                        case "Customer":
                            await customerService.CleanupCustomerDataAsync(user.Id);
                            break;
                        case "SaleManager":
                            await DeleteEntityIfExistsAsync(salesManagerRepo, sm => sm.UserId == user.Id);
                            break;
                        case "TechManager":
                            await DeleteEntityIfExistsAsync(techManagerRepo, tm => tm.UserId == user.Id);
                            break;
                        case "StockControlManager":
                            await DeleteEntityIfExistsAsync(stockControlManagerRepo, scm => scm.UserId == user.Id);
                            break;
                        case "Admin":
                            await DeleteEntityIfExistsAsync(adminRepo, a => a.UserId == user.Id);
                            break;
                        case "TechCompany":
                            await DeleteEntityIfExistsAsync(techCompanyRepo, tc => tc.UserId == user.Id);
                            break;
                    }

                    await userManager.RemoveFromRoleAsync(user, roleName);
                }

                await context.SaveChangesAsync();

                var deleteResult = await userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Failed to delete user account: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}",
                        Data = null
                    };
                }

                await transaction.CommitAsync();

                return AuthMapper.MapToDeleteAccountResponse(true, "Your account has been deleted successfully.", user.Email);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred while deleting account: {ex.Message}",
                    Data = null
                };
            }
        }

        private async Task DeleteEntityIfExistsAsync<TEntity>(IRepository<TEntity> repo, Expression<Func<TEntity, bool>> predicate)
            where TEntity : BaseEntity
        {
            var entity = (await repo.FindAsync(predicate)).FirstOrDefault();
            if (entity != null) repo.Remove(entity);
        }

        public async Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Invalid credentials. User not found.",
                    Data = null
                };
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Invalid credentials. Password is incorrect.",
                    Data = null
                };
            }

            var token = GenerateJwtToken(user);
            var roles = (await userManager.GetRolesAsync(user)).ToList();

            var loginResultDTO = new LoginResultDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                RoleName = roles
            };

            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Customer":
                        var customer = await customerRepo.GetFirstOrDefaultAsync(c => c.UserId == user.Id);
                        if (customer == null) return FailRoleWarning("Customer", user.Id);
                        loginResultDTO.CustomerId = customer.Id;
                        // Fetch CartId
                        var cart = await cartRepo.GetFirstOrDefaultAsync(c => c.CustomerId == customer.Id);
                        if (cart != null)
                            loginResultDTO.CartId = cart.Id;
                        // Fetch WishListId
                        var wishListsResponse = await wishListService.GetByCustomerIdAsync(customer.Id);
                        var wishList = wishListsResponse.Data?.FirstOrDefault();
                        if (wishList != null)
                            loginResultDTO.WishListId = wishList.Id;
                        break;

                    case "SaleManager":
                        var salesManager = await salesManagerRepo.GetFirstOrDefaultAsync(sm => sm.UserId == user.Id);
                        if (salesManager == null) return FailRoleWarning("SaleManager", user.Id);
                        loginResultDTO.SalesManagerId = salesManager.Id;
                        break;

                    case "TechManager":
                        var techManager = await techManagerRepo.GetFirstOrDefaultAsync(tm => tm.UserId == user.Id);
                        if (techManager == null) return FailRoleWarning("TechManager", user.Id);
                        loginResultDTO.TechManagerId = techManager.Id;
                        break;

                    case "StockControlManager":
                        var stockManager = await stockControlManagerRepo.GetFirstOrDefaultAsync(sc => sc.UserId == user.Id);
                        if (stockManager == null) return FailRoleWarning("StockControlManager", user.Id);
                        loginResultDTO.StockControlManagerId = stockManager.Id;
                        break;

                    case "Admin":
                        var admin = await adminRepo.GetFirstOrDefaultAsync(a => a.UserId == user.Id);
                        if (admin == null) return FailRoleWarning("Admin", user.Id);
                        loginResultDTO.AdminId = admin.Id;
                        break;

                    case "TechCompany":
                        var techCompany = await techCompanyRepo.GetFirstOrDefaultAsync(tc => tc.UserId == user.Id);
                        if (techCompany == null) return FailRoleWarning("TechCompany", user.Id);
                        loginResultDTO.TechCompanyId = techCompany.Id;
                        break;
                }
            }

            return new GeneralResponse<LoginResultDTO>
            {
                Success = true,
                Message = "User logged in successfully.",
                Data = loginResultDTO
            };
        }

        public async Task<GeneralResponse<string>> RegisterAsync(RegisterDTO dto)
        {
            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User registration failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await context.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = user.Id
            };
        }

        public async Task<GeneralResponse<string>> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found",
                    Data = $"User email '{dto.Email}' is not registered."
                };
            }

            var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password reset failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = user.Id
            };
        }

        public async Task<GeneralResponse<string>> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found",
                    Data = $"user email {dto.Email.ToString()} not registered"
                };
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Password reset token generated successfully.",
                Data = token
            };
        }
        private GeneralResponse<LoginResultDTO> FailRoleWarning(string role, string userId)
        {
            return new GeneralResponse<LoginResultDTO>
            {
                Success = false,
                Message = $"Warning: {role} role assigned to UserId {userId}, but no {role} profile found during login.",
                Data = null
            };
        }
        private string GenerateJwtToken(AppUser user)
        {
            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
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
