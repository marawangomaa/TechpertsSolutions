using Core.DTOs.CategoryDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<CartItem> _cartItemRepository;

        public CategoryService(IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<CartItem> cartItemRepository) 
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<GeneralResponse<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllWithIncludesAsync(
                    c => c.SubCategories,
                    c => c.Products
                );

                // Use CategoryMapper for mapping
                var mappedCategories = CategoryMapper.MapToCategoryDTOList(categories);
                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully.",
                    Data = mappedCategories ?? Enumerable.Empty<CategoryDTO>()
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving categories.",
                    Data = null
                };
            }
        }

        // Retrieves a single category by ID, including its sub-categories and products.
        public async Task<GeneralResponse<CategoryDTO>> GetCategoryByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var category = await _categoryRepository.GetByIdWithIncludesAsync(
                    id,
                    c => c.SubCategories,
                    c => c.Products
                );

                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{id}' not found.",
                        Data = null
                    };
                }

                // Use CategoryMapper for mapping
                var mappedCategory = CategoryMapper.MapToCategoryDTO(category);
                if (mappedCategory == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = "Failed to map category data.",
                        Data = null
                    };
                }
                
                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    Data = mappedCategory
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the category.",
                    Data = null
                };
            }
        }

        // Creates a new category from a DTO.
        public async Task<GeneralResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO categoryCreateDto)
        {
            // Input validation
            if (categoryCreateDto == null)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(categoryCreateDto.Name))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category name is required.",
                    Data = null
                };
            }

            try
            {
                // Use CategoryMapper for mapping
                var category = CategoryMapper.MapToCategory(categoryCreateDto);

                await _categoryRepository.AddAsync(category); // Add entity via repository
                await _categoryRepository.SaveChangesAsync(); // Save changes

                // Fetch the created category with includes for complete response
                var createdCategory = await _categoryRepository.GetByIdWithIncludesAsync(
                    category.Id,
                    c => c.SubCategories,
                    c => c.Products
                );

                // Use CategoryMapper for mapping
                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category created successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(createdCategory)
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the category.",
                    Data = null
                };
            }
        }

        // Updates an existing category from a DTO.
        public async Task<GeneralResponse<CategoryDTO>> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDto)
        {
            // Input validation
            if (categoryUpdateDto == null)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(categoryUpdateDto.Id))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(categoryUpdateDto.Id, out _))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(categoryUpdateDto.Name))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category name is required.",
                    Data = null
                };
            }

            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryUpdateDto.Id);
                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{categoryUpdateDto.Id}' not found.",
                        Data = null
                    };
                }

                // Use CategoryMapper for mapping
                CategoryMapper.MapToCategory(categoryUpdateDto, category);

                _categoryRepository.Update(category); // Mark entity as updated
                await _categoryRepository.SaveChangesAsync(); // Save changes

                // Fetch the updated category with includes for complete response
                var updatedCategory = await _categoryRepository.GetByIdWithIncludesAsync(
                    category.Id,
                    c => c.SubCategories,
                    c => c.Products
                );
                
                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category updated successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(updatedCategory)
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the category.",
                    Data = null
                };
            }
        }

        // Deletes a category by ID.
        public async Task<GeneralResponse<bool>> DeleteCategoryAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Category ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
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
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Category with ID '{id}' not found.",
                        Data = false
                    };
                }

                // --- Start of changes for manual deletion ---

                // 1. Find all products associated with this category
                // Use FindAsync from productRepository for filtering
                var productsToDelete = await _productRepository.FindAsync(p => p.CategoryId == id);

                // 2. For each product, find and delete its associated cart items
                foreach (var product in productsToDelete)
                {
                    var cartItemsToDelete = await _cartItemRepository.FindAsync(ci => ci.ProductId == product.Id);
                    // If your IRepository<T> doesn't have a RemoveRange, you'll need to loop:
                    foreach (var cartItem in cartItemsToDelete)
                    {
                        _cartItemRepository.Remove(cartItem);
                    }
                    // If you added RemoveRange to IRepository, you could use:
                    // _cartItemRepository.RemoveRange(cartItemsToDelete.ToList());
                }

                // 3. Remove the products
                // Again, if no RemoveRange, loop:
                foreach (var product in productsToDelete)
                {
                    _productRepository.Remove(product);
                }
                // If you added RemoveRange to IRepository, you could use:
                // _productRepository.RemoveRange(productsToDelete.ToList());

                // --- End of changes for manual deletion ---

                // 4. Finally, remove the category itself
                _categoryRepository.Remove(category);

                // 5. Save all changes. This single SaveChangesAsync call will commit
                // all pending deletions (cart items, then products, then category)
                // within a single transaction, resolving the foreign key conflict.
                await _categoryRepository.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Category deleted successfully.",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the category.",
                    Data = false
                };
            }
        }
    }
}
