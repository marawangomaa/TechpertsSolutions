using Core.DTOs.WarrantyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IWarrantyService
    {
        Task<GeneralResponse<WarrantyReadDTO>> CreateAsync(WarrantyCreateDTO dto);
        Task<GeneralResponse<WarrantyReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<WarrantyReadDTO>>> GetAllAsync();
        Task<GeneralResponse<WarrantyReadDTO>> UpdateAsync(string id, WarrantyUpdateDTO dto);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
    }
}
