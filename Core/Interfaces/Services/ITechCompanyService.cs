using Core.DTOs.TechCompanyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ITechCompanyService
    {
        Task<GeneralResponse<TechCompanyReadDTO>> CreateAsync(TechCompanyCreateDTO dto);
        Task<GeneralResponse<TechCompanyReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<TechCompanyReadDTO>>> GetAllAsync();
        Task<GeneralResponse<TechCompanyReadDTO>> UpdateAsync(string id, TechCompanyUpdateDTO dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
    }
}
