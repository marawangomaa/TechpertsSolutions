using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

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
        private readonly IRepository<WishList> wishListRepo;
        private readonly TechpertsContext context;
        private readonly ICustomerService customerService;
        private readonly ITechCompanyService techCompanyService;

        public RoleService(
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            IRepository<Admin> adminRepo,
            IRepository<Customer> customerRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            IRepository<Cart> cartRepo,
            IRepository<WishList> wishListRepo,
            TechpertsContext context,
            ICustomerService customerService,
            ITechCompanyService techCompanyService
        )
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
            this.techCompanyService = techCompanyService;
        }

        public async Task<GeneralResponse<bool>> CheckRoleAsync(string roleName)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            return RoleMapper.MapToRoleCheckResponse(exists);
        }

        public async Task<GeneralResponse<object>> AssignRoleAsync(
            string userEmail,
            RoleType roleName
        )
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
                        Data = $"{userEmail} is not registered",
                    };

                var result = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
                if (!result.Succeeded)
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Failed to assign role",
                        Data = string.Join(" ", result.Errors.Select(e => e.Description)),
                    };

                var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
                if (role == null)
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = $"Role '{roleName}' does not exist.",
                        Data = $"{roleName} not found",
                    };

                await AddDomainEntityAsync(roleName, user.Id, role.Id);
                await transaction.CommitAsync();

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                    Data = $"Role '{roleName}' assigned to user '{userEmail}' successfully.",
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning the role.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<string>> UnassignRoleAsync(
            string userEmail,
            RoleType roleName
        )
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = userEmail,
                };
            }

            if (!await userManager.IsInRoleAsync(user, roleName.GetStringValue()))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"User is not assigned to role '{roleName}'.",
                    Data = $"{userEmail} is not assigned as {roleName}",
                };
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName.GetStringValue());
            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to remove role '{roleName}' from user.",
                    Data = string.Join(" ", result.Errors.Select(e => e.Description)),
                };
            }

            await RemoveDomainEntityAsync(roleName, user.Id);

            return new GeneralResponse<string>
            {
                Success = true,
                Message =
                    $"Role '{roleName}' unassigned and domain entity removed for user '{userEmail}'.",
                Data = $"Role {roleName} no longer assigned to {userEmail}",
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

                        var savedCustomer = await customerRepo.GetFirstOrDefaultAsync(c =>
                            c.UserId == userId
                        );

                        var newCart = new Cart
                        {
                            CustomerId = savedCustomer.Id,
                            CreatedAt = DateTime.Now,
                        };
                        await cartRepo.AddAsync(newCart);

                        var newWishList = new WishList
                        {
                            CustomerId = savedCustomer.Id,
                            CreatedAt = DateTime.Now,
                        };
                        await wishListRepo.AddAsync(newWishList);
                        await wishListRepo.SaveChangesAsync();
                    }
                    break;

                case RoleType.Admin:
                    // Check if admin already exists
                    if (!await adminRepo.AnyAsync(a => a.UserId == userId))
                    {
                        var admin = new Admin { UserId = userId, RoleId = roleId };
                        await adminRepo.AddAsync(admin);
                    }
                    break;

                case RoleType.TechCompany:
                    if (!await techCompanyRepo.AnyAsync(tc => tc.UserId == userId))
                        await techCompanyRepo.AddAsync(
                            new TechCompany { UserId = userId, RoleId = roleId }
                        );
                    break;

                case RoleType.DeliveryPerson:
                    if (!await deliveryPersonRepo.AnyAsync(dp => dp.UserId == userId))
                        await deliveryPersonRepo.AddAsync(
                            new DeliveryPerson { UserId = userId, RoleId = roleId }
                        );
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

                    case RoleType.TechCompany:
                        await techCompanyService.CleanupTechCompanyDataAsync(userId);
                        break;

                    case RoleType.Admin:
                        var admin = await adminRepo.GetFirstOrDefaultAsync(a => a.UserId == userId);
                        if (admin != null)
                            adminRepo.Remove(admin);
                        break;

                    case RoleType.DeliveryPerson:
                        var dp = await deliveryPersonRepo.GetFirstOrDefaultAsync(x =>
                            x.UserId == userId
                        );
                        if (dp != null)
                            deliveryPersonRepo.Remove(dp);
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
