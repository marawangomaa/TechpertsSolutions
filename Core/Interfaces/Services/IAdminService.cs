using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.DTOs.AdminDTOs;

namespace Core.Interfaces.Services
{
    public interface IAdminService
    {
        Task<GeneralResponse<IEnumerable<AdminReadDTO>>> GetAllAsync();
        Task<GeneralResponse<AdminReadDTO>> GetByIdAsync(string id);
    }
}
