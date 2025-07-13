using Core.DTOs.Admin;
using Core.Interfaces;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<AdminReadDTO> CreateAsync(AdminCreateDTO dto)
        {
            var admin = new Admin() 
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId,
            };
            await adminRepository.AddAsync(admin);
            await adminRepository.SaveChanges();

            return await GetByIdAsync(admin.Id);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var admin = await adminRepository.GetByIdAsync(id);
            if (admin == null) return false;

            adminRepository.Remove(admin);
            await adminRepository.SaveChanges();

            return true;
        }

        public async Task<IEnumerable<AdminReadDTO>> GetAllAsync()
        {
            var admins = await adminRepository.GetAllAsync();
            return admins.Select(AdminMapper.AdminReadDTOMapper).ToList();
        }

        public async Task<AdminReadDTO> GetByIdAsync(string id)
        {
            var admin = await adminRepository.GetByIdAsync(id);
            if (admin == null) return null;
            return AdminMapper.AdminReadDTOMapper(admin);
        }

        public async Task<bool> UpdateRoleAsync(string id, AdminUpdateDTO dto)
        {
            var admin = await adminRepository.GetByIdAsync(id);
            if (admin == null) return false;
            admin.RoleId = dto.RoleId;
            adminRepository.Update(admin);
            await adminRepository.SaveChanges();
            return true;
        }
    }
}
