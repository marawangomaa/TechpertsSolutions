using Core.DTOs.AdminDTOs;
using Core.DTOs;
using Core.DTOs.ProductDTOs;

namespace Core.Interfaces.Services
{
    public interface IAdminUserManagementService
    {
        Task<GeneralResponse<PaginatedDTO<UserListDTO>>> GetAllUsersAsync(
            int pageNumber, int pageSize, string? search, string? role, bool? isActive, string? sortBy, bool sortDesc);
        Task<GeneralResponse<UserListDTO>> GetUserByIdAsync(string userId);
        Task<GeneralResponse<string>> DeactivateUserAsync(string userId);
        Task<GeneralResponse<string>> ActivateUserAsync(string userId);
        Task<GeneralResponse<List<string>>> ChangeUserRolesAsync(string userId, List<string> newRoles);
        Task<GeneralResponse<object>> GetUserStatisticsAsync();
    }
} 
