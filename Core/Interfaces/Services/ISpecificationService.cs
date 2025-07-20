using Core.DTOs.Specifications;
using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISpecificationService
    {
        Task<GeneralResponse<IEnumerable<SpecificationDTO>>> GetAllSpecificationsAsync();
        Task<GeneralResponse<SpecificationDTO>> GetSpecificationByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<SpecificationDTO>>> GetSpecificationsByProductIdAsync(string productId);
        Task<GeneralResponse<SpecificationDTO>> CreateSpecificationAsync(CreateSpecificationDTO createDto);
        Task<GeneralResponse<bool>> UpdateSpecificationAsync(UpdateSpecificationDTO updateDto);
        Task<GeneralResponse<bool>> DeleteSpecificationAsync(string id);
    }
}
