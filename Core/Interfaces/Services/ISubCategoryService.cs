using Core.DTOs.SubCategory;
using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISubCategoryService
    {
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetAllSubCategoriesAsync();
        Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByIdAsync(string id);
        Task<GeneralResponse<SubCategoryDTO>> CreateSubCategoryAsync(CreateSubCategoryDTO createDto);
        Task<GeneralResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryDTO updateDto);
        Task<GeneralResponse<bool>> DeleteSubCategoryAsync(string id);
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetSubCategoriesByCategoryIdAsync(string categoryId);
    }
}
