using Core.DTOs.SubCategory;
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



        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesAsync()
        {
            try
            {
                // Include Category to get CategoryName for DTO mapping
                var subCategories = await _subCategoryRepository.GetAllWithIncludesAsync(sc => sc.Category);

                return SubCategoryMapper.MapToSubCategoryDTOList(subCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all subcategories.");
                throw;
            }
        }

        public async Task<SubCategoryDTO?> GetSubCategoryByIdAsync(string id)
        {
            try
            {
                // Include Category to get CategoryName for DTO mapping
                var subCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(id, sc => sc.Category);
                return SubCategoryMapper.MapToSubCategoryDTO(subCategory);
            }
            catch (KeyNotFoundException)
            {
                return null; // SubCategory not found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting subcategory with ID: {id}");
                throw;
            }
        }

        public async Task<SubCategoryDTO> CreateSubCategoryAsync(CreateSubCategoryDTO createDto)
        {
            try
            {
                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == createDto.CategoryId);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Category with ID '{createDto.CategoryId}' does not exist.");
                }

                // Use SubCategoryMapper for mapping
                var subCategory = SubCategoryMapper.MapToSubCategory(createDto);

                await _subCategoryRepository.AddAsync(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                // Fetch the created subcategory with its category for the DTO response
                var createdSubCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(subCategory.Id, sc => sc.Category);
                return SubCategoryMapper.MapToSubCategoryDTO(createdSubCategory);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subcategory.");
                throw;
            }
        }

        public async Task<bool> UpdateSubCategoryAsync(UpdateSubCategoryDTO updateDto)
        {
            try
            {
                var existingSubCategory = await _subCategoryRepository.GetByIdAsync(updateDto.Id);
                if (existingSubCategory == null)
                {
                    return false; // SubCategory not found
                }

                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == updateDto.CategoryId);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Category with ID '{updateDto.CategoryId}' does not exist.");
                }

                // Use SubCategoryMapper for mapping
                SubCategoryMapper.MapToSubCategory(updateDto, existingSubCategory);

                _subCategoryRepository.Update(existingSubCategory);
                await _subCategoryRepository.SaveChangesAsync();
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating subcategory with ID: {updateDto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteSubCategoryAsync(string id)
        {
            try
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(id);
                if (subCategory == null)
                {
                    return false; // SubCategory not found
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting subcategory with ID: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<SubCategoryDTO>> GetSubCategoriesByCategoryIdAsync(string categoryId)
        {
            try
            {
                // Validate if the CategoryId exists
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Category with ID '{categoryId}' does not exist.");
                }

                // Find subcategories by CategoryId and include the Category for DTO mapping
                var subCategories = await _subCategoryRepository.FindWithIncludesAsync(
                    sc => sc.CategoryId == categoryId,
                    sc => sc.Category
                );

                return SubCategoryMapper.MapToSubCategoryDTOList(subCategories);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting subcategories for Category ID: {categoryId}");
                throw;
            }
        }
    }
}
