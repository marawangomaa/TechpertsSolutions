using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs;
using Core.DTOs.ProductDTOs;

namespace Core.Interfaces.Services
{
    public interface IPCAssemblyCompatibilityService
    {
        Task<GeneralResponse<PaginatedDTO<CompatibleProductDTO>>> GetCompatiblePartsAsync(
            CompatibilityFilterDTO filter, int pageNumber, int pageSize);
        Task<GeneralResponse<PaginatedDTO<CompatibleProductDTO>>> GetComponentsByFilterAsync(
            CompatibilityFilterDTO filter, int pageNumber, int pageSize);
        Task<GeneralResponse<PaginatedDTO<CompatibleProductDTO>>> GetCompatiblePartsByCategoryAsync(
            string categoryId, int pageNumber, int pageSize);
        Task<GeneralResponse<bool>> CheckCompatibilityAsync(string productId1, string productId2);
    }
} 
