using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.MaintenanceDTOs;

namespace Core.Interfaces.Services
{
    public interface IMaintenanceService
    {
        Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAllAsync();
        Task<GeneralResponse<MaintenanceDetailsDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<MaintenanceDTO>> AddAsync(MaintenanceCreateDTO dto);
        Task<GeneralResponse<bool>> UpdateAsync(string id, MaintenanceUpdateDTO dto);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetByTechCompanyIdAsync(string techCompanyId);
        Task<GeneralResponse<MaintenanceDTO>> AcceptMaintenanceRequestAsync(string maintenanceId, string techCompanyId);
        Task<GeneralResponse<MaintenanceDTO>> CompleteMaintenanceAsync(string maintenanceId, string techCompanyId, string notes);
        Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAvailableMaintenanceRequestsAsync();
        Task<GeneralResponse<MaintenanceDTO>> UpdateMaintenanceStatusAsync(string maintenanceId, string newStatus);
        Task<GeneralResponse<MaintenanceNearestDTO>> GetNearestMaintenanceAsync(string customerId);
    }
}
