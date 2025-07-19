using Core.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(string Id);
        Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO categoryDTO);
        Task<bool> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO);
        Task<bool> DeleteCategoryAsync(string Id);
    }
}
