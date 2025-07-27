using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Service.Utilities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using Core.Entities;

namespace Service
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IRepository<Admin> adminRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<DeliveryPerson> deliveryPersonRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<TechpertsSolutions.Core.Entities.WishList> wishListRepo;
        private readonly TechpertsContext context;
        private readonly ICustomerService customerService;

        public RoleService(
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            IRepository<Admin> adminRepo,
            IRepository<Customer> customerRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            IRepository<Cart> cartRepo,
            IRepository<TechpertsSolutions.Core.Entities.WishList> wishListRepo,
            TechpertsContext context,
            ICustomerService customerService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.adminRepo = adminRepo;
            this.customerRepo = customerRepo;
            this.techCompanyRepo = techCompanyRepo;
            this.deliveryPersonRepo = deliveryPersonRepo;
            this.cartRepo = cartRepo;
            this.wishListRepo = wishListRepo;
            this.context = context;
            this.customerService = customerService;
        }

        public async Task<GeneralResponse<bool>> CheckRoleAsync(string roleName)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            return RoleMapper.MapToRoleCheckResponse(exists);
        }

        public async Task<GeneralResponse<object>> AssignRoleAsync(string userEmail, RoleType roleName)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "User not found.",
                        Data = $"{userEmail} is not registered"
                    };

                var result = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
                if (!result.Succeeded)
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Failed to assign role",
                        Data = string.Join(" ", result.Errors.Select(e => e.Description))
                    };

                var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
                if (role == null)
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = $"Role '{roleName}' does not exist.",
                        Data = $"{roleName} not found"
                    };

                if (roleName == RoleType.Customer)
                {
                    await AddDomainEntityAsync(roleName, user.Id, role.Id);
                    await context.SaveChangesAsync();
                }
                else
                {
                    await AddDomainEntityAsync(roleName, user.Id, role.Id);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                    Data = $"Role '{roleName}' assigned to user '{userEmail}' successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning the role.",
                    Data = ex.Message
                };
            }
        }

        public async Task<GeneralResponse<string>> UnassignRoleAsync(string userEmail, RoleType roleName)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = userEmail
                };
            }

            if (!await userManager.IsInRoleAsync(user, roleName.GetStringValue()))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"User is not assigned to role '{roleName}'.",
                    Data = $"{userEmail} is not assigned as {roleName}"
                };
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to remove role '{roleName}' from user.",
                    Data = string.Join(" ", result.Errors.Select(e => e.Description))
                };
            }

            await RemoveDomainEntityAsync(roleName, user.Id);
            await context.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = $"Role '{roleName}' unassigned and domain entity removed for user '{userEmail}'.",
                Data = $"Role {roleName} no longer assigned to {userEmail}"
            };
        }

        public Task<GeneralResponse<List<string>>> GetAllRolesAsync()
        {
            var roles = roleManager.Roles.ToList();
            return Task.FromResult(RoleMapper.MapToRoleListResponse(roles));
        }

        private async Task AddDomainEntityAsync(RoleType roleName, string userId, string roleId)
        {
            switch (roleName)
            {
                case RoleType.Customer:
                    if (!await customerRepo.AnyAsync(c => c.UserId == userId))
                    {
                        var newCustomer = new Customer { UserId = userId, RoleId = roleId };
                        await customerRepo.AddAsync(newCustomer);
                        await context.SaveChangesAsync();

                        // Reload the customer to ensure Id is set
                        var savedCustomer = await customerRepo.GetFirstOrDefaultAsync(c => c.UserId == userId);

                        var newCart = new Cart { CustomerId = savedCustomer.Id, CreatedAt = DateTime.UtcNow };
                        await cartRepo.AddAsync(newCart);

                        // Create WishList for the new customer (using repo)
                        var newWishList = new TechpertsSolutions.Core.Entities.WishList { CustomerId = savedCustomer.Id, CreatedAt = DateTime.UtcNow };
                        await wishListRepo.AddAsync(newWishList);
                        await wishListRepo.SaveChangesAsync();
                    }
                    break;

                case RoleType.Admin:
                    if (!await adminRepo.AnyAsync(a => a.UserId == userId))
                        await adminRepo.AddAsync(new Admin { UserId = userId, RoleId = roleId });
                    break;

                case RoleType.TechCompany:
                    if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == userId))
                        await techCompanyRepo.AddAsync(new TechCompany { UserId = userId, RoleId = roleId });
                    break;

                case RoleType.DeliveryPerson:
                    if (!await deliveryPersonRepo.AnyAsync(dp => dp.UserId == userId))
                        await deliveryPersonRepo.AddAsync(new DeliveryPerson { UserId = userId, RoleId = roleId });
                    break;
            }
        }

        private async Task RemoveDomainEntityAsync(RoleType roleName, string userId)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                switch (roleName)
                {
                    case RoleType.Customer:
                        await customerService.CleanupCustomerDataAsync(userId);
                        break;

                    case RoleType.Admin:
                        var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                        if (admin != null) adminRepo.Remove(admin);
                        break;

                    case RoleType.TechCompany:
                        var tc = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                        if (tc != null) techCompanyRepo.Remove(tc);
                        break;

                    case RoleType.DeliveryPerson:
                        var dp = (await deliveryPersonRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                        if (dp != null) deliveryPersonRepo.Remove(dp);
                        break;
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
