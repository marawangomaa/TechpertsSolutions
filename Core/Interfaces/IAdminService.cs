using Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminReadDTO>> GetAllAsync();
        Task<AdminReadDTO> GetByIdAsync(int id);
        Task<AdminReadDTO> CreateAsync(AdminCreateDTO dto);
        Task<bool> UpdateRoleAsync(int id, AdminUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
