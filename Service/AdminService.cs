using Core.DTOs.Admin;
using TechpertsSolutions.Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
                var admins = await adminRepository.GetAllWithIncludesAsync(a=>a.User, a => a.Role);
                return new GeneralResponse<IEnumerable<AdminReadDTO>>
                {
                    Success = true,
                    Message = "Admins retrieved successfully.",
                    Data = admins.Select(AdminMapper.AdminReadDTOMapper).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<AdminReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving admins.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<AdminReadDTO>> GetByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "Admin ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "Invalid Admin ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var admin = await adminRepository.GetByIdWithIncludesAsync(id, a => a.User, a => a.Role);
                if (admin == null)
                {
                    return new GeneralResponse<AdminReadDTO>
                    {
                        Success = false,
                        Message = $"Admin with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<AdminReadDTO>
                {
                    Success = true,
                    Message = "Admin retrieved successfully.",
                    Data = AdminMapper.AdminReadDTOMapper(admin)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<AdminReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the admin.",
                    Data = null
                };
            }
        }
    }
}
