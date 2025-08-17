using Core.DTOs;
using Core.DTOs.CustomerDTOs;
using Core.DTOs.ProfileDTOs;
using Core.DTOs.ServiceUsageDTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<GeneralResponse<string>> UpdateProfileAsync(string userId,UserProfileUpdateDTO dto);
        Task<GeneralResponse<string>> UploadProfilePhotoAsync(string userId, IFormFile photoFile);
        Task<GeneralResponse<UserServiceUsageDTO>> GetUserServicesUsageAsync(string userId);
        Task<GeneralResponse<UserProfileDTO>> GetUserProfileAsync(string userId);
        Task<GeneralResponse<string>> UpdateUserProfileAsync(string userId,UserProfileUpdateDTO dto);
        Task<GeneralResponse<string>> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordDTO dto);
        Task<GeneralResponse<string>> UpdateUserLocationAsync(string userId, UserLocationUpdateDTO dto);
    }
} 
