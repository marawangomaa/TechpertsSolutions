using Core.DTOs.TechManagerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ITechManagerService
    {
        Task<GeneralResponse<TechManagerReadDTO>> CreateAsync(TechManagerCreateDTO dto);
        Task<GeneralResponse<TechManagerReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<TechManagerReadDTO>>> GetAllAsync();
        Task<GeneralResponse<TechManagerReadDTO>> UpdateAsync(string id, TechManagerUpdateDTO dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
    }
}
