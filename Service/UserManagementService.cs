using System.Security.Claims;
using Core.DTOs;
using Core.DTOs.CustomerDTOs;
using Core.DTOs.ProfileDTOs;
using Core.DTOs.ServiceUsageDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IRepository<ServiceUsage> _serviceUsageRepository;
        private readonly IRepository<PCAssembly> _pcAssemblyRepository;
        private readonly IRepository<Maintenance> _maintenanceRepository;

        public UserManagementService(
            UserManager<AppUser> userManager,
            IFileService fileService,
            IRepository<ServiceUsage> serviceUsageRepository,
            IRepository<PCAssembly> pcAssemblyRepository,
            IRepository<Maintenance> maintenanceRepository
        )
        {
            _userManager = userManager;
            _fileService = fileService;
            _serviceUsageRepository = serviceUsageRepository;
            _pcAssemblyRepository = pcAssemblyRepository;
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<GeneralResponse<string>> UpdateProfileAsync(
            string userId,
            UserProfileUpdateDTO dto
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                // Update user properties
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    user.FullName = dto.FullName;
                if (!string.IsNullOrWhiteSpace(dto.Email))
                    user.Email = dto.Email;
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    user.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    user.Address = dto.Address;
                user.UpdatedAt = DateTime.Now;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to update profile.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Profile updated successfully.",
                    Data = userId,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating profile.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<UserProfileDTO>> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<UserProfileDTO>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userProfile = new UserProfileDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    ProfilePhotoUrl = user.ProfilePhotoUrl,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    PostalCode = user.PostalCode,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                };

                return new GeneralResponse<UserProfileDTO>
                {
                    Success = true,
                    Message = "User profile retrieved successfully.",
                    Data = userProfile,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserProfileDTO>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user profile.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<string>> UpdateUserProfileAsync(
            string userId,
            UserProfileUpdateDTO dto
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                // Update user properties
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    user.FullName = dto.FullName;
                if (!string.IsNullOrWhiteSpace(dto.Email))
                    user.Email = dto.Email;
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    user.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    user.Address = dto.Address;
                if (!string.IsNullOrWhiteSpace(dto.City))
                    user.City = dto.City;
                if (!string.IsNullOrWhiteSpace(dto.Country))
                    user.Country = dto.Country;

                user.UpdatedAt = DateTime.Now;

                // Handle profile photo upload if provided
                if (dto.ProfilePhoto != null && dto.ProfilePhoto.Length > 0)
                {
                    var photoUrl = await _fileService.UploadImageAsync(
                        dto.ProfilePhoto,
                        "profiles"
                    );
                    if (!string.IsNullOrEmpty(photoUrl))
                    {
                        user.ProfilePhotoUrl = photoUrl;
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to update profile.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Profile updated successfully.",
                    Data = userId,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating profile.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<string>> UploadProfilePhotoAsync(
            string userId,
            IFormFile photoFile
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                // Upload the photo
                var photoUrl = await _fileService.UploadImageAsync(photoFile, "profiles");
                if (string.IsNullOrEmpty(photoUrl))
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to upload photo.",
                    };
                }

                // Update user's profile photo URL
                user.ProfilePhotoUrl = photoUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to update profile photo.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Profile photo uploaded successfully.",
                    Data = photoUrl,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while uploading profile photo.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<UserServiceUsageDTO>> GetUserServicesUsageAsync(
            string userId
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<UserServiceUsageDTO>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                // Get service usage counts
                var maintenanceCount = await _maintenanceRepository.GetAllAsync();
                maintenanceCount = maintenanceCount.Where(m => m.CustomerId == userId).ToList();

                var pcAssemblyCount = await _pcAssemblyRepository.GetAllAsync();
                pcAssemblyCount = pcAssemblyCount.Where(p => p.CustomerId == userId).ToList();

                // For delivery, we'll count orders with delivery
                // This would need to be implemented based on your delivery tracking logic

                var services = new List<ServiceUsageSummaryDTO>
                {
                    new ServiceUsageSummaryDTO
                    {
                        ServiceId = 1,
                        ServiceName = "Maintenance",
                        UsageCount = maintenanceCount.Count(),
                    },
                    new ServiceUsageSummaryDTO
                    {
                        ServiceId = 2,
                        ServiceName = "PC Build",
                        UsageCount = pcAssemblyCount.Count(),
                    },
                    new ServiceUsageSummaryDTO
                    {
                        ServiceId = 3,
                        ServiceName = "Delivery",
                        UsageCount = 0, // This would need to be calculated based on delivery tracking
                    },
                };

                var result = new UserServiceUsageDTO
                {
                    UserId = userId,
                    UserName = user.FullName,
                    Services = services,
                };

                return new GeneralResponse<UserServiceUsageDTO>
                {
                    Success = true,
                    Message = "Service usage retrieved successfully.",
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserServiceUsageDTO>
                {
                    Success = false,
                    Message = "An error occurred while retrieving service usage.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<string>> ChangePasswordAsync(
            ClaimsPrincipal principal,
            ChangePasswordDTO dto
        )
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };
            }
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "New password and confirmation do not match.",
                    Data = null,
                };
            }
            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Password change failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                };
            }

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Password changed successfully.",
                Data = user.Id,
            };
        }

        public async Task<GeneralResponse<string>> UpdateUserLocationAsync(
            string userId,
            UserLocationUpdateDTO dto
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                bool updated = false;

                if (!string.IsNullOrWhiteSpace(dto.PostalCode))
                {
                    user.PostalCode = dto.PostalCode;
                    updated = true;
                }
                if (dto.Latitude.HasValue)
                {
                    user.Latitude = dto.Latitude.Value;
                    updated = true;
                }
                if (dto.Longitude.HasValue)
                {
                    user.Longitude = dto.Longitude.Value;
                    updated = true;
                }

                if (!updated)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "No location fields provided to update.",
                    };
                }

                user.UpdatedAt = DateTime.Now;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to update location.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Location updated successfully.",
                    Data = user.Id,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating location.",
                    Data = ex.Message,
                };
            }
        }
    }
}
