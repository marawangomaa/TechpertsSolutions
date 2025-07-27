using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.CategoryDTOs;

namespace Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<GeneralResponse<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync();
        Task<GeneralResponse<CategoryDTO>> GetCategoryByIdAsync(string Id);
        Task<GeneralResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO categoryDTO);
        Task<GeneralResponse<CategoryDTO>> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO);
        Task<GeneralResponse<bool>> DeleteCategoryAsync(string Id);
    }
}
