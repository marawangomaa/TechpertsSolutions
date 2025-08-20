using Core.DTOs;
using Core.DTOs.ServiceUsageDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IServiceUsageService
    {
        Task<GeneralResponse<ServiceUsageReadDTO>> CreateAsync(ServiceUsageCreateDTO dto);
        Task<GeneralResponse<ServiceUsageReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetAllAsync();
        Task<GeneralResponse<ServiceUsageReadDTO>> UpdateAsync(string id, ServiceUsageUpdateDTOs dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
        Task<GeneralResponse<ServiceUsageReadDTO>> TrackServiceUsageAsync(string customerId, ServiceType serviceType, string? techCompanyId = null);
        Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetServiceUsageByCustomerAsync(string customerId);
        Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetServiceUsageByTechCompanyAsync(string techCompanyId);
        Task<GeneralResponse<ServiceUsageReadDTO>> GetOrCreateServiceUsageAsync(string customerId, ServiceType serviceType);
    }
}
