using Core.DTOs.PCAssemblyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IPCAssemblyService
    {
        Task<GeneralResponse<PCAssemblyReadDTO>> CreateAsync(PCAssemblyCreateDTO dto);
        Task<GeneralResponse<PCAssemblyReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetAllAsync();
        Task<GeneralResponse<PCAssemblyReadDTO>> UpdatePCAssemblyAsync(string id, PCAssemblyUpdateDTO dto);
        Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetByCustomerIdAsync(string customerId);
        
        
        Task<GeneralResponse<PCAssemblyReadDTO>> AddComponentToAssemblyAsync(string assemblyId, PCAssemblyItemCreateDTO item);
        Task<GeneralResponse<PCAssemblyReadDTO>> RemoveComponentFromAssemblyAsync(string assemblyId, string itemId);
        Task<GeneralResponse<PCBuildStatusDTO>> GetPCBuildStatusAsync(string assemblyId);
        Task<GeneralResponse<PCBuildTotalDTO>> CalculateBuildTotalAsync(string assemblyId);
        Task<GeneralResponse<IEnumerable<CompatibleComponentDTO>>> GetCompatibleComponentsAsync(string productId);
    }
}
