using Core.DTOs.WishListDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
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

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IRepository<Admin> adminRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<Cart> cartRepo;
        private readonly IRepository<TechCompany> techCompanyRepo;
        private readonly IRepository<DeliveryPerson> deliveryPersonRepo;
        private readonly ICustomerService customerService;
        private readonly IWishListService wishListService;
        private readonly IPCAssemblyService pcAssemblyService;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly TechpertsContext context;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IRepository<Admin> adminRepo,
            IRepository<Customer> customerRepo,
            IRepository<Cart> cartRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            ICustomerService customerService,
            IWishListService wishListService,
            IPCAssemblyService pcAssemblyService,
            IEmailService emailService,
            IConfiguration configuration,
            TechpertsContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.adminRepo = adminRepo;
            this.customerRepo = customerRepo;
            this.cartRepo = cartRepo;
            this.techCompanyRepo = techCompanyRepo;
            this.deliveryPersonRepo = deliveryPersonRepo;
            this.customerService = customerService;
            this.wishListService = wishListService;
            this.pcAssemblyService = pcAssemblyService;
            this.emailService = emailService;
            this.configuration = configuration;
            this.context = context;
        }

        public async Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDTO)
        {
            // Input validation
            if (loginDTO == null)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Login data is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Email))
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Email is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Password is required.",
                    Data = null
                };
            }

            try
            {
                // Normalize email (trim and convert to lowercase)
                var normalizedEmail = loginDTO.Email.Trim().ToLowerInvariant();

                var user = await userManager.FindByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "Invalid email or password.",
                        Data = null
                    };
                }

                // Check if user is locked out
                if (await userManager.IsLockedOutAsync(user))
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "Account is temporarily locked. Please try again later.",
                        Data = null
                    };
                }

                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (!isPasswordValid)
                {
                    // Increment failed login attempts
                    await userManager.AccessFailedAsync(user);
                    
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "Invalid email or password.",
                        Data = null
                    };
                }

                // Reset failed login attempts on successful login
                await userManager.ResetAccessFailedCountAsync(user);

                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "User has no assigned roles. Please contact administrator.",
                        Data = null
                    };
                }

                var token = GenerateJwtToken(user, roles);

                var loginResultDTO = new LoginResultDTO
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = roles,
                    PCAssemblyId = null // Will be set below if customer has PC assemblies
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
                            // Fetch PCAssemblyId (get the most recent one)
                            var pcAssembliesResponse = await pcAssemblyService.GetByCustomerIdAsync(customer.Id);
                            var latestPCAssembly = pcAssembliesResponse.Data?.OrderByDescending(pc => pc.CreatedAt).FirstOrDefault();
                            if (latestPCAssembly != null)
                                loginResultDTO.PCAssemblyId = latestPCAssembly.Id;
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

                        case "DeliveryPerson":
                            var deliveryPerson = await deliveryPersonRepo.GetFirstOrDefaultAsync(dp => dp.UserId == user.Id);
                            if (deliveryPerson == null) return FailRoleWarning("DeliveryPerson", user.Id);
                            loginResultDTO.DeliveryPersonId = deliveryPerson.Id;
                            break;
                    }
                }

                return new GeneralResponse<LoginResultDTO>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = loginResultDTO
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<string>> RegisterAsync(RegisterDTO registerDTO,RoleType roleName)
        {
            // Input validation
            if (registerDTO == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Registration data is required.",
                    Data = null
                };
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Email is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.ConfirmPassword))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password confirmation is required.",
                    Data = null
                };
            }

            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Passwords do not match.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.FullName))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Full name is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.UserName))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Username is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.Address))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Address is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Phone number is required.",
                    Data = null
                };
            }

            try
            {
                // Normalize email (trim and convert to lowercase)
                var normalizedEmail = registerDTO.Email.Trim().ToLowerInvariant();

                var existingUser = await userManager.FindByEmailAsync(normalizedEmail);
                if (existingUser != null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Email is already registered.",
                        Data = null
                    };
                }

                // Check if username is already taken
                var existingUserByUsername = await userManager.FindByNameAsync(registerDTO.UserName.Trim());
                if (existingUserByUsername != null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Username is already taken.",
                        Data = null
                    };
                }

                var user = new AppUser
                {
                    UserName = registerDTO.UserName.Trim(),
                    Email = normalizedEmail,
                    FullName = registerDTO.FullName.Trim(),
                    Address = registerDTO.Address.Trim(),
                    PhoneNumber = registerDTO.PhoneNumber.Trim(),
                    EmailConfirmed = false, // Require email confirmation
                    PhoneNumberConfirmed = false
                };

                var result = await userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Registration failed: {errors}",
                        Data = null
                    };
                }

                // Get the role name from the enum
                //var roleName = registerDTO.Role.GetStringValue();
                var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
                if (role == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Role '{roleName}' not found.",
                        Data = null
                    };
                }

                // Assign the selected role
                var roleResult = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Failed to assign role: {errors}",
                        Data = null
                    };
                }

                // Create the appropriate entity based on the role
                string? entityId = null;
                string? cartId = null;

                switch (roleName)
                {
                    case RoleType.Customer:
                        var customer = new Customer
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        };
                        await customerRepo.AddAsync(customer);
                        await context.SaveChangesAsync();

                        // Create cart for the new customer
                        var cart = new Cart
                        {
                            CustomerId = customer.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        await cartRepo.AddAsync(cart);

                        // Create wishlist for the new customer
                        await wishListService.CreateAsync(new WishListCreateDTO { CustomerId = customer.Id });

                        entityId = customer.Id;
                        cartId = cart.Id;
                        break;

                    case RoleType.Admin:
                        var admin = new Admin
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        };
                        await adminRepo.AddAsync(admin);
                        entityId = admin.Id;
                        break;

                    case RoleType.TechCompany:
                        var techCompany = new TechCompany
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        };
                        await techCompanyRepo.AddAsync(techCompany);
                        entityId = techCompany.Id;
                        break;

                    case RoleType.DeliveryPerson:
                        var deliveryPerson = new DeliveryPerson
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        };
                        await deliveryPersonRepo.AddAsync(deliveryPerson);
                        entityId = deliveryPerson.Id;
                        break;
                }

                await context.SaveChangesAsync();

                // Generate login result
                var roles = await userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                var loginResultDTO = new LoginResultDTO
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = roles,
                    CustomerId = roleName == RoleType.Customer ? entityId : null,
                    CartId = cartId,
                    PCAssemblyId = null // New customers won't have PC assemblies yet
                };

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = $"Registration successful. User registered as {roleName}.",
                    Data = $"Registration successful. User registered as {roleName}."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again.",
                    Data = null
                };
            }
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

        public async Task<GeneralResponse<string>> DeleteAccountAsync(DeleteAccountDTO dto, string userId)
        {
            // Input validation
            if (dto == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required to delete account.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required to delete account.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User ID is required.",
                    Data = null
                };
            }

            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                // Verify password before deletion
                var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Invalid password. Account deletion failed.",
                        Data = null
                    };
                }

                // Delete user (this will cascade delete related entities)
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Account deletion failed: {errors}",
                        Data = null
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Your account has been deleted successfully.",
                    Data = user.Email
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting your account. Please try again.",
                    Data = null
                };
            }
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private GeneralResponse<LoginResultDTO> FailRoleWarning(string roleName, string userId)
        {
            return new GeneralResponse<LoginResultDTO>
            {
                Success = false,
                Message = $"User has '{roleName}' role but no corresponding entity found. UserId: {userId}",
                Data = null
            };
        }
    }
}
