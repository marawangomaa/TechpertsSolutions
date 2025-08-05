using Core.DTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.CartDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IPCAssemblyService
    {
        Task<GeneralResponse<PCAssemblyReadDTO>> CreateAsync(PCAssemblyCreateDTO dto);
        Task<GeneralResponse<PCAssemblyReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetAllAsync();
        Task<GeneralResponse<PCAssemblyReadDTO>> UpdatePCAssemblyAsync(string id, PCAssemblyUpdateDTO dto);
        Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetByCustomerIdAsync(string customerId);
        Task<GeneralResponse<IEnumerable<PCAssemblyItemReadDTO>>> GetAllComponentsAsync(string assemblyId);
        Task<PCAssemblyItemReadDTO?> GetComponentByCategoryAsync(string assemblyId, ProductCategory category);
        Task<GeneralResponse<PCAssemblyReadDTO>> AddComponentToAssemblyAsync(string assemblyId, PCAssemblyItemCreateDTO item);
        Task<GeneralResponse<PCAssemblyReadDTO>> RemoveComponentFromAssemblyAsync(string assemblyId, string itemId);
        Task<GeneralResponse<PCBuildStatusDTO>> GetPCBuildStatusAsync(string assemblyId);
        Task<GeneralResponse<PCBuildTotalDTO>> CalculateBuildTotalAsync(string assemblyId);
        Task<GeneralResponse<IEnumerable<CompatibleComponentDTO>>> GetCompatibleProductsForCategoryAsync(string assemblyId, string categoryName);
        Task<GeneralResponse<CartReadDTO>> SaveBuildToCartAsync(string assemblyId, string customerId, decimal assemblyFee);
        Task<GeneralResponse<PCBuildTableDTO>> GetPCBuildTableAsync(string assemblyId);
    }
}
