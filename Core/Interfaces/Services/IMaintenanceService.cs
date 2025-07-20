using Core.DTOs.Maintenance;
using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IMaintenanceService
    {
        Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAllAsync();
        Task<GeneralResponse<MaintenanceDetailsDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<MaintenanceDTO>> AddAsync(MaintenanceCreateDTO dto);
        Task<GeneralResponse<bool>> UpdateAsync(string id, MaintenanceUpdateDTO dto);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
    }
}
