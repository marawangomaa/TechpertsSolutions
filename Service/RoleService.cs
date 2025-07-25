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
        private readonly IRepository<SalesManager> salesManagerRepo;
        private readonly IRepository<StockControlManager> stockControlManagerRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<TechManager> techManagerRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<TechpertsSolutions.Core.Entities.WishList> wishListRepo;
        private readonly TechpertsContext context;
        private readonly ICustomerService customerService;

        public RoleService(
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            IRepository<Admin> adminRepo,
            IRepository<Customer> customerRepo,
            IRepository<SalesManager> salesManagerRepo,
            IRepository<StockControlManager> stockControlManagerRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<TechManager> techManagerRepo,
            IRepository<Cart> cartRepo,
            IRepository<TechpertsSolutions.Core.Entities.WishList> wishListRepo,
            TechpertsContext context,
            ICustomerService customerService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.adminRepo = adminRepo;
            this.customerRepo = customerRepo;
            this.salesManagerRepo = salesManagerRepo;
            this.stockControlManagerRepo = stockControlManagerRepo;
            this.techCompanyRepo = techCompanyRepo;
            this.techManagerRepo = techManagerRepo;
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

            // For Customer, capture the created IDs
            if (roleName == RoleType.Customer)
            {
                // AddDomainEntityAsync will create the customer, cart, and wishlist
                await AddDomainEntityAsync(roleName, user.Id, role.Id);
                await context.SaveChangesAsync();

                // Fetch the created entities
                var savedCustomer = await customerRepo.GetFirstOrDefaultAsync(c => c.UserId == user.Id);
                var cart = (await cartRepo.GetAllAsync()).FirstOrDefault(ct => ct.CustomerId == savedCustomer.Id);
                var wishList = (await wishListRepo.GetAllAsync()).FirstOrDefault(wl => wl.CustomerId == savedCustomer.Id);

                var dto = new CustomerRoleAssignmentResultDTO
                {
                    CustomerId = savedCustomer?.Id,
                    CartId = cart?.Id,
                    WishListId = wishList?.Id
                };

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                    Data = dto
                };
            }
            else
            {
                await AddDomainEntityAsync(roleName, user.Id, role.Id);
                await context.SaveChangesAsync();
                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                    Data = null
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
                case RoleType.SaleManager:
                    if (!await salesManagerRepo.AnyAsync(sm => sm.UserId == userId))
                        await salesManagerRepo.AddAsync(new SalesManager { UserId = userId, RoleId = roleId });
                    break;

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

                case RoleType.TechManager:
                    if (!await techManagerRepo.AnyAsync(t => t.UserId == userId))
                        await techManagerRepo.AddAsync(new TechManager { UserId = userId, RoleId = roleId });
                    break;

                case RoleType.StockControlManager:
                    if (!await stockControlManagerRepo.AnyAsync(s => s.UserId == userId))
                        await stockControlManagerRepo.AddAsync(new StockControlManager { UserId = userId, RoleId = roleId });
                    break;

                case RoleType.Admin:
                    if (!await adminRepo.AnyAsync(a => a.UserId == userId))
                        await adminRepo.AddAsync(new Admin { UserId = userId, RoleId = roleId });
                    break;

                case RoleType.TechCompany:
                    if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == userId))
                        await techCompanyRepo.AddAsync(new TechCompany { UserId = userId, RoleId = roleId });
                    break;
            }
        }

        private async Task RemoveDomainEntityAsync(RoleType roleName, string userId)
        {
            switch (roleName)
            {
                case RoleType.Customer:
                    await customerService.CleanupCustomerDataAsync(userId);
                    break;

                case RoleType.SaleManager:
                    var sm = (await salesManagerRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                    if (sm != null) salesManagerRepo.Remove(sm);
                    break;

                case RoleType.TechManager:
                    var tm = (await techManagerRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                    if (tm != null) techManagerRepo.Remove(tm);
                    break;

                case RoleType.StockControlManager:
                    var scm = (await stockControlManagerRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                    if (scm != null) stockControlManagerRepo.Remove(scm);
                    break;

                case RoleType.Admin:
                    var admin = (await adminRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                    if (admin != null) adminRepo.Remove(admin);
                    break;

                case RoleType.TechCompany:
                    var tc = (await techCompanyRepo.GetAllAsync()).FirstOrDefault(x => x.UserId == userId);
                    if (tc != null) techCompanyRepo.Remove(tc);
                    break;
            }
        }
    }
}
