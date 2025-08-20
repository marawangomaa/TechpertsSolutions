using Core.DTOs;
using Core.DTOs.AdminDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Admin> adminRepository;

        public AdminService(IRepository<Admin> _adminRepository)
        {
            adminRepository = _adminRepository;
        }

        public async Task<GeneralResponse<IEnumerable<AdminReadDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for admin listing with user and role information
                var admins = await adminRepository.GetAllWithIncludesAsync(
                    a => a.User,
                    a => a.Role
                );

                var adminDtos = admins.Select(AdminMapper.MapToAdminReadDTO).ToList();

                return new GeneralResponse<IEnumerable<AdminReadDTO>>
                {
                    Success = true,
                    Message = "Admins retrieved successfully.",
                    Data = adminDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<AdminReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving admins.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<AdminReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "Admin ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "Invalid Admin ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                // Comprehensive includes for detailed admin view with user and role information
                var admin = await adminRepository.GetByIdWithIncludesAsync(
                    id,
                    a => a.User,
                    a => a.Role
                );

                if (admin == null)
                {
                    return new GeneralResponse<AdminReadDTO>
                    {
                        Success = false,
                        Message = $"Admin with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<AdminReadDTO>
                {
                    Success = true,
                    Message = "Admin retrieved successfully.",
                    Data = AdminMapper.MapToAdminReadDTO(admin),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the admin.",
                    Data = null,
                };
            }
        }
    }
}
