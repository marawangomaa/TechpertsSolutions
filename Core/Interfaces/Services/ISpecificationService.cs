using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.ProductDTOs;
using Core.DTOs.SpecificationsDTOs;
using SpecificationDTO = Core.DTOs.ProductDTOs.SpecificationDTO;

namespace Core.Interfaces.Services
{
    public interface ISpecificationService
    {
        Task<GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>> GetAllSpecificationsAsync();
        Task<GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>> GetSpecificationByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>> GetSpecificationsByProductIdAsync(string productId);
        Task<GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>> CreateSpecificationAsync(CreateSpecificationDTO createDto);
        Task<GeneralResponse<bool>> UpdateSpecificationAsync(UpdateSpecificationDTO updateDto);
        Task<GeneralResponse<bool>> DeleteSpecificationAsync(string id);
        Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationAsync(string key, string value);
        Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationIdAsync(string specificationId);
    }
}
