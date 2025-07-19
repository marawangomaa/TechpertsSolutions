using Core.DTOs.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISpecificationService
    {
        Task<IEnumerable<SpecificationDTO>> GetAllSpecificationsAsync();
        Task<SpecificationDTO?> GetSpecificationByIdAsync(string id);
        Task<IEnumerable<SpecificationDTO>> GetSpecificationsByProductIdAsync(string productId);
        Task<SpecificationDTO?> CreateSpecificationAsync(CreateSpecificationDTO createDto);
        Task<bool> UpdateSpecificationAsync(UpdateSpecificationDTO updateDto);
        Task<bool> DeleteSpecificationAsync(string id);
    }
}
