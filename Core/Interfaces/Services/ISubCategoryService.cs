using Core.DTOs.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesAsync();
        Task<SubCategoryDTO?> GetSubCategoryByIdAsync(string id);
        Task<SubCategoryDTO> CreateSubCategoryAsync(CreateSubCategoryDTO createDto);
        Task<bool> UpdateSubCategoryAsync(UpdateSubCategoryDTO updateDto);
        Task<bool> DeleteSubCategoryAsync(string id);
        Task<IEnumerable<SubCategoryDTO>> GetSubCategoriesByCategoryIdAsync(string categoryId);
    }
}
