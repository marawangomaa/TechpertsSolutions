using Core.DTOs.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceDTO>> GetAllAsync();
        Task<MaintenanceDetailsDTO?> GetByIdAsync(string id);
        Task<MaintenanceDTO> AddAsync(MaintenanceCreateDTO dto);
        Task<bool> UpdateAsync(string id, MaintenanceUpdateDTO dto);
        Task<bool> DeleteAsync(string id);
    }
}
