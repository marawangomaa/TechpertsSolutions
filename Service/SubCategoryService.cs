using Core.DTOs.SubCategoryDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<Product> _productRepository; // Needed for manual cascading delete
        private readonly IRepository<Category> _categoryRepository; // To validate CategoryId
        private readonly ILogger<SubCategoryService> _logger;

        public SubCategoryService(
            IRepository<SubCategory> subCategoryRepository,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            ILogger<SubCategoryService> logger)
        {
            _subCategoryRepository = subCategoryRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetAllSubCategoriesAsync()
        {
            try
            {
                // Include Category and Products to get complete data for DTO mapping
                var subCategories = await _subCategoryRepository.GetAllWithIncludesAsync(
                    sc => sc.Category,
                    sc => sc.Products
                );

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTOList(subCategories)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all subcategories.");
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving subcategories.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Include Category and Products to get complete data for DTO mapping
                var subCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(
                    id, 
                    sc => sc.Category,
                    sc => sc.Products
                );
                
                if (subCategory == null)
                {
                    return new GeneralResponse<SubCategoryDTO>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = true,
                    Message = "SubCategory retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTO(subCategory)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting subcategory with ID: {id}");
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the subcategory.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<SubCategoryDTO>> CreateSubCategoryAsync(CreateSubCategoryDTO createDto)
        {
            // Input validation
            if (createDto == null)
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory name is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(createDto.CategoryId))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "Category ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(createDto.CategoryId, out _))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == createDto.CategoryId);
                if (!categoryExists)
                {
                    return new GeneralResponse<SubCategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{createDto.CategoryId}' does not exist.",
                        Data = null
                    };
                }

                // Use SubCategoryMapper for mapping
                var subCategory = SubCategoryMapper.MapToSubCategory(createDto);

                await _subCategoryRepository.AddAsync(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                // Fetch the created subcategory with its category for the DTO response
                var createdSubCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(
                    subCategory.Id, 
                    sc => sc.Category,
                    sc => sc.Products
                );
                
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = true,
                    Message = "SubCategory created successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTO(createdSubCategory)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subcategory.");
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the subcategory.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryDTO updateDto)
        {
            // Input validation
            if (updateDto == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(updateDto.Id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(updateDto.Id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(updateDto.Name))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory name is required.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(updateDto.CategoryId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Category ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(updateDto.CategoryId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var existingSubCategory = await _subCategoryRepository.GetByIdAsync(updateDto.Id);
                if (existingSubCategory == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{updateDto.Id}' not found.",
                        Data = false
                    };
                }

                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == updateDto.CategoryId);
                if (!categoryExists)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Category with ID '{updateDto.CategoryId}' does not exist.",
                        Data = false
                    };
                }

                // Use SubCategoryMapper for mapping
                SubCategoryMapper.MapToSubCategory(updateDto, existingSubCategory);

                _subCategoryRepository.Update(existingSubCategory);
                await _subCategoryRepository.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "SubCategory updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating subcategory with ID: {updateDto.Id}");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the subcategory.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteSubCategoryAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(id);
                if (subCategory == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found.",
                        Data = false
                    };
                }

                // --- Manual Cascading Delete for Products associated with this SubCategory ---
                // This assumes DeleteBehavior.Cascade is NOT configured in DbContext for SubCategory -> Product
                // If it IS configured, this section can be removed.
                var productsToDelete = await _productRepository.FindAsync(p => p.SubCategoryId == id);
                if (productsToDelete != null && productsToDelete.Any())
                {
                    // As noted before, if you are doing manual cascading, you would also need to
                    // fetch and delete/nullify the dependents of these products (CartItems, OrderItems, etc.)
                    // before deleting the products themselves. This is simplified here for brevity.
                    _productRepository.RemoveRange(productsToDelete.ToList());
                }

                _subCategoryRepository.Remove(subCategory);
                await _subCategoryRepository.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "SubCategory deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting subcategory with ID: {id}");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the subcategory.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetSubCategoriesByCategoryIdAsync(string categoryId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Category ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(categoryId, out _))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                    {
                        Success = false,
                        Message = $"Category with ID '{categoryId}' does not exist.",
                        Data = null
                    };
                }

                // Find subcategories by CategoryId and include the Category and Products for DTO mapping
                var subCategories = await _subCategoryRepository.FindWithIncludesAsync(
                    sc => sc.CategoryId == categoryId,
                    sc => sc.Category,
                    sc => sc.Products
                );

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTOList(subCategories)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting subcategories for Category ID: {categoryId}");
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving subcategories.",
                    Data = null
                };
            }
        }
    }
}
