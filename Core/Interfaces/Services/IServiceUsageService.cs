using Core.DTOs.ServiceUsage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IServiceUsageService
    {
        Task<GeneralResponse<ServiceUsageReadDTO>> CreateAsync(ServiceUsageCreateDTO dto);
        Task<GeneralResponse<ServiceUsageReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetAllAsync();
        Task<GeneralResponse<ServiceUsageReadDTO>> UpdateAsync(string id, ServiceUsageUpdateDTO dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
    }
}
