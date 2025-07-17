using Core.DTOs.Admin;
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

        public async Task<IEnumerable<AdminReadDTO>> GetAllAsync()
        {
            var admins = await adminRepository.GetAllWithIncludesAsync(a=>a.User, a => a.Role);
            return admins.Select(AdminMapper.AdminReadDTOMapper).ToList();
        }

        public async Task<AdminReadDTO?> GetByIdAsync(string id)
        {
            var admin = await adminRepository.GetByIdWithIncludesAsync(id, a => a.User, a => a.Role);
            if (admin == null) return null;
            return AdminMapper.AdminReadDTOMapper(admin);
        }
    }
}
