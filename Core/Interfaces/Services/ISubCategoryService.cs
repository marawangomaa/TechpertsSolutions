using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.SubCategoryDTOs;
using Microsoft.AspNetCore.Http;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ISubCategoryService
    {
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetAllSubCategoriesAsync();
        Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByIdAsync(string id);
        Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByNameAsync(string name);
        Task<GeneralResponse<SubCategoryDTO>> CreateSubCategoryAsync(CreateSubCategoryDTO createDto);
        Task<GeneralResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryDTO updateDto);
        Task<GeneralResponse<bool>> DeleteSubCategoryAsync(string id);
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetSubCategoriesByCategoryIdAsync(string categoryId);
        Task<GeneralResponse<ImageUploadResponseDTO>> UploadSubCategoryImageAsync(IFormFile imageFile, string subCategoryId);
        Task<GeneralResponse<bool>> DeleteSubCategoryImageAsync(string subCategoryId);
        Task<GeneralResponse<object>> AssignSubCategoryToCategoryAsync(string subCategoryId, string categoryId);
        Task<GeneralResponse<object>> AssignSubCategoryToMultipleCategoriesAsync(AssignSubCategoryToCategoriesDTO assignDto);
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetUnassignedSubCategoriesAsync();
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetSubCategoriesWithIncludesAsync(string includeProperties = null);
        Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetSubCategoriesByCategoryIdWithIncludesAsync(string categoryId, string includeProperties = null);
    }
}
