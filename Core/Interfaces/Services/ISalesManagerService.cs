using Core.DTOs.SalesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ISalesManagerService
    {
        Task<GeneralResponse<SalesManagerReadDTO>> CreateAsync(SalesManagerCreateDTO dto);
        Task<GeneralResponse<SalesManagerReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<SalesManagerReadDTO>>> GetAllAsync();
        Task<GeneralResponse<SalesManagerReadDTO>> UpdateAsync(string id, SalesManagerUpdateDTO dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
    }
}
