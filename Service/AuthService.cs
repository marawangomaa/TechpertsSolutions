using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Core.DTOs;
using Core.DTOs.WishListDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly INotificationService notificationService;
        private readonly ICustomerService customerService;
        private readonly IWishListService wishListService;
        private readonly IPCAssemblyService pcAssemblyService;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly TechpertsContext context;
        private readonly IFileService fileService;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IRepository<Admin> adminRepo,
            IRepository<Customer> customerRepo,
            IRepository<Cart> cartRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            INotificationService notificationService,
            ICustomerService customerService,
            IWishListService wishListService,
            IPCAssemblyService pcAssemblyService,
            IEmailService emailService,
            IConfiguration configuration,
            TechpertsContext context,
            IFileService fileService
        )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.adminRepo = adminRepo;
            this.customerRepo = customerRepo;
            this.cartRepo = cartRepo;
            this.techCompanyRepo = techCompanyRepo;
            this.deliveryPersonRepo = deliveryPersonRepo;
            this.notificationService = notificationService;
            this.customerService = customerService;
            this.wishListService = wishListService;
            this.pcAssemblyService = pcAssemblyService;
            this.emailService = emailService;
            this.configuration = configuration;
            this.context = context;
            this.fileService = fileService;
        }

        public async Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Login data is required.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Email))
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Email is required.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Password is required.",
                    Data = null,
                };
            }

            try
            {
                var normalizedEmail = loginDTO.Email.Trim().ToLowerInvariant();

                var user = await userManager.FindByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = $"No Email Found.",
                        Data = null,
                    };
                }

                if (await userManager.IsLockedOutAsync(user))
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "Account is temporarily locked. Please try again later.",
                        Data = null,
                    };
                }

                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (!isPasswordValid)
                {
                    await userManager.AccessFailedAsync(user);

                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "Incorrect password.",
                        Data = null,
                    };
                }

                await userManager.ResetAccessFailedCountAsync(user);

                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    return new GeneralResponse<LoginResultDTO>
                    {
                        Success = false,
                        Message = "User has no assigned roles. Please contact administrator.",
                        Data = null,
                    };
                }

                var token = GenerateJwtToken(user, roles);

                var loginResultDTO = new LoginResultDTO
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = roles,
                    PCAssemblyId = null,
                    ProfilePhotoUrl = !string.IsNullOrEmpty(user.ProfilePhotoUrl)
                        ? user.ProfilePhotoUrl
                        : "/assets/profiles/default-profile.jpg",
                };

                foreach (var role in roles)
                {
                    switch (role)
                    {
                        case "Customer":
                            var customer = await customerRepo.GetFirstOrDefaultAsync(c =>
                                c.UserId == user.Id
                            );
                            if (customer == null)
                                return FailRoleWarning("Customer", user.Id);
                            loginResultDTO.CustomerId = customer.Id;

                            var cart = await cartRepo.GetFirstOrDefaultAsync(c =>
                                c.CustomerId == customer.Id
                            );
                            if (cart != null)
                                loginResultDTO.CartId = cart.Id;

                            var wishListsResponse = await wishListService.GetByCustomerIdAsync(
                                customer.Id
                            );
                            var wishList = wishListsResponse.Data?.FirstOrDefault();
                            if (wishList != null)
                                loginResultDTO.WishListId = wishList.Id;

                            var pcAssembliesResponse = await pcAssemblyService.GetByCustomerIdAsync(
                                customer.Id
                            );
                            var latestPCAssembly = pcAssembliesResponse
                                .Data?.OrderByDescending(pc => pc.CreatedAt)
                                .FirstOrDefault();
                            if (latestPCAssembly != null)
                                loginResultDTO.PCAssemblyId = latestPCAssembly.Id;
                            break;

                        case "Admin":
                            var admin = await adminRepo.GetFirstOrDefaultAsync(a =>
                                a.UserId == user.Id
                            );
                            if (admin == null)
                                return FailRoleWarning("Admin", user.Id);
                            loginResultDTO.AdminId = admin.Id;
                            break;

                        case "TechCompany":
                            var techCompany = await techCompanyRepo.GetFirstOrDefaultAsync(tc =>
                                tc.UserId == user.Id
                            );
                            if (techCompany == null)
                                return FailRoleWarning("TechCompany", user.Id);
                            loginResultDTO.TechCompanyId = techCompany.Id;
                            break;

                        case "DeliveryPerson":
                            var deliveryPerson = await deliveryPersonRepo.GetFirstOrDefaultAsync(
                                dp => dp.UserId == user.Id
                            );
                            if (deliveryPerson == null)
                                return FailRoleWarning("DeliveryPerson", user.Id);
                            loginResultDTO.DeliveryPersonId = deliveryPerson.Id;
                            break;
                    }
                }
                await notificationService.SendNotificationAsync(
                    user.Id,
                    NotificationType.SystemAlert,
                    null,
                    "User",
                    $"New login detected for your account on {DateTime.Now:yyyy-MM-dd HH:mm}."
                );

                return new GeneralResponse<LoginResultDTO>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = loginResultDTO,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = $"An error occurred during login. Please try again.{ex}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<string>> RegisterAsync(
            RegisterDTO registerDTO,
            RoleType roleName
        )
        {
            if (registerDTO == null)
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Registration data is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.Email))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Email is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.Password))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.ConfirmPassword))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password confirmation is required.",
                    Data = null,
                };

            if (registerDTO.Password != registerDTO.ConfirmPassword)
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Passwords do not match.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.FullName))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Full name is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.UserName))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Username is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.Address))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Address is required.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Phone number is required.",
                    Data = null,
                };

            try
            {
                var normalizedEmail = registerDTO.Email.Trim().ToLowerInvariant();

                if (await userManager.FindByEmailAsync(normalizedEmail) != null)
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Email is already registered.",
                        Data = null,
                    };

                if (await userManager.FindByNameAsync(registerDTO.UserName.Trim()) != null)
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Username is already taken.",
                        Data = null,
                    };

                var user = new AppUser
                {
                    UserName = registerDTO.UserName.Trim(),
                    Email = normalizedEmail,
                    FullName = registerDTO.FullName.Trim(),
                    Address = registerDTO.Address.Trim(),
                    PhoneNumber = registerDTO.PhoneNumber.Trim(),
                    City = registerDTO.City?.Trim(),
                    Country = registerDTO.Country?.Trim(),
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    CreatedAt = DateTime.Now,
                };

                if (registerDTO.ProfilePhoto != null && registerDTO.ProfilePhoto.Length > 0)
                {
                    var photoUrl = await fileService.UploadImageAsync(
                        registerDTO.ProfilePhoto,
                        "profiles"
                    );
                    user.ProfilePhotoUrl = photoUrl;
                }

                var result = await userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message =
                            $"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}",
                        Data = null,
                    };

                var role = await roleManager.FindByNameAsync(roleName.GetStringValue());
                if (role == null)
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Role '{roleName}' not found.",
                        Data = null,
                    };

                var roleResult = await userManager.AddToRoleAsync(user, roleName.GetStringValue());
                if (!roleResult.Succeeded)
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message =
                            $"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}",
                        Data = null,
                    };

                string? entityId = null;
                string? cartId = null;

                switch (roleName)
                {
                    case RoleType.Customer:
                        var customer = new Customer { UserId = user.Id, RoleId = role.Id };
                        await customerRepo.AddAsync(customer);
                        await context.SaveChangesAsync();

                        var cart = new Cart { CustomerId = customer.Id, CreatedAt = DateTime.Now };
                        await cartRepo.AddAsync(cart);
                        await wishListService.CreateAsync(
                            new WishListCreateDTO { CustomerId = customer.Id }
                        );

                        entityId = customer.Id;
                        cartId = cart.Id;
                        break;

                    case RoleType.Admin:
                        var admin = new Admin { UserId = user.Id, RoleId = role.Id };
                        await adminRepo.AddAsync(admin);
                        entityId = admin.Id;
                        break;

                    case RoleType.TechCompany:
                        var techCompany = new TechCompany { UserId = user.Id, RoleId = role.Id };
                        await techCompanyRepo.AddAsync(techCompany);
                        entityId = techCompany.Id;
                        break;

                    case RoleType.DeliveryPerson:
                        var deliveryPerson = new DeliveryPerson
                        {
                            UserId = user.Id,
                            RoleId = role.Id,
                        };
                        await deliveryPersonRepo.AddAsync(deliveryPerson);
                        entityId = deliveryPerson.Id;
                        break;
                }

                await context.SaveChangesAsync();

                var roles = await userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                await notificationService.SendNotificationToRoleAsync(
                    "Admin",
                    NotificationType.SystemAlert,
                    entityId,
                    "User",
                    $"New user '{user.FullName}' has registered with role '{roleName}'."
                );

                await notificationService.SendNotificationAsync(
                    user.Id,
                    NotificationType.SystemAlert,
                    entityId,
                    "User",
                    $"Welcome {user.FullName}! Your account has been created successfully."
                );

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = $"Registration successful. User registered as {roleName}.",
                    Data = $"Registration successful. User registered as {roleName}.",
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred during registration. Please try again.{ex}",
                    Data = null,
                };
            }
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
                    Data = null,
                };
            }

            // Generate JWT with claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("purpose", "reset-password"),
            };

            var jwtToken = GenerateJwtToken(claims, TimeSpan.FromMinutes(15));
            var angularAppUrl = configuration["AppSettings:FrontendUrl"]?.TrimEnd('/');
            var resetLink =
                $"{angularAppUrl}/reset-password?token={HttpUtility.UrlEncode(jwtToken)}";

            var emailSent = await emailService.SendPasswordResetEmailAsync(
                user.Email,
                jwtToken,
                resetLink
            );
            if (!emailSent)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to send reset email.",
                    Data = null,
                };
            }

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Password reset email sent successfully.",
                Data = jwtToken,
            };
        }

        public async Task<GeneralResponse<string>> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var principal = ValidateJwtToken(dto.Token);
            if (principal == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or expired token.",
                    Data = null,
                };
            }

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var purpose = principal.FindFirstValue("purpose");

            if (purpose != "reset-password")
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid token purpose.",
                    Data = null,
                };
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };
            }

            // Generate the official Identity reset token internally
            var identityToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, identityToken, dto.NewPassword);

            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password reset failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                };
            }

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = user.Id,
            };
        }

        public async Task<GeneralResponse<string>> DeleteAccountAsync(
            DeleteAccountDTO dto,
            string userId
        )
        {
            if (dto == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required to delete account.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password is required to delete account.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User ID is required.",
                    Data = null,
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
                        Data = null,
                    };
                }

                var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Invalid password. Account deletion failed.",
                        Data = null,
                    };
                }

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Account deletion failed: {errors}",
                        Data = null,
                    };
                }
                await notificationService.SendNotificationToRoleAsync(
                    "Admin",
                    NotificationType.SystemAlert,
                    $"Account Have Been Deleted."
                );

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Your account has been deleted successfully.",
                    Data = user.Email,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting your account. Please try again.",
                    Data = null,
                };
            }
        }

        // PRIVATE - Generate JWT from AppUser + roles
        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            // Add roles as "role" claims (so ASP.NET Core picks them up correctly)
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            return GenerateJwtToken(claims, TimeSpan.FromDays(7));
        }

        // PRIVATE - Generate JWT from claims
        private string GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan expiry)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // PRIVATE - Validate JWT
        private ClaimsPrincipal? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        ),
                        RoleClaimType = "role",
                    },
                    out _
                );

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private GeneralResponse<LoginResultDTO> FailRoleWarning(string roleName, string userId)
        {
            return new GeneralResponse<LoginResultDTO>
            {
                Success = false,
                Message =
                    $"User has '{roleName}' role but no corresponding entity found. UserId: {userId}",
                Data = null,
            };
        }
    }
}
