using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Http;
using Core.DTOs;
using Core.DTOs.SubCategoryDTOs;

namespace Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<GeneralResponse<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync();
        Task<GeneralResponse<CategoryDTO>> GetCategoryByIdAsync(string Id);
        Task<GeneralResponse<CategoryDTO>> GetCategoryByNameAsync(string name);
        Task<GeneralResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO categoryDTO);
        Task<GeneralResponse<CategoryDTO>> UpdateCategoryAsync(string id,CategoryUpdateDTO categoryUpdateDTO);
        Task<GeneralResponse<bool>> DeleteCategoryAsync(string Id);
        Task<GeneralResponse<ImageUploadResponseDTO>> UploadCategoryImageAsync(IFormFile imageFile, string categoryId);
        Task<GeneralResponse<bool>> DeleteCategoryImageAsync(string categoryId);
        Task<GeneralResponse<object>> AssignSubCategoryToCategoryAsync(string categoryId, AssignSubCategoryDTO assignDto);
        Task<GeneralResponse<object>> AssignMultipleSubCategoriesToCategoryAsync(string categoryId, List<string> subCategoryIds);
    }
}
