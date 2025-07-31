using Core.DTOs.CustomerDTOs;
using Core.DTOs.ServiceUsageDTOs;
using Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<GeneralResponse<string>> UpdateProfileAsync(string userId, UserProfileUpdateDTO dto);
        Task<GeneralResponse<string>> UploadProfilePhotoAsync(string userId, IFormFile photoFile);
        Task<GeneralResponse<UserServiceUsageDTO>> GetUserServicesUsageAsync(string userId);
        Task<GeneralResponse<UserProfileDTO>> GetUserProfileAsync(string userId);
        Task<GeneralResponse<string>> UpdateUserProfileAsync(string userId, UserProfileUpdateDTO dto);
    }
} 
