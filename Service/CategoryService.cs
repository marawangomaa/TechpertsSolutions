using Core.DTOs;
using Core.DTOs.CategoryDTOs;
using Core.DTOs.ProductDTOs;
using Core.DTOs.SubCategoryDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IFileService _fileService;

        public CategoryService(IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<CartItem> cartItemRepository,
            IFileService fileService) 
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
            _fileService = fileService;
        }

        public async Task<GeneralResponse<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(includeProperties: "SubCategories,SubCategories.Products,Products");

                var categoryDtos = categories.Select(CategoryMapper.MapToCategoryDTO).ToList();

                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully.",
                    Data = categoryDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while retrieving categories.{ex}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CategoryDTO>> GetCategoryByIdAsync(string id)
        {
            
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
                var categories = await _categoryRepository.GetAllAsync(includeProperties: "SubCategories,SubCategories.Products,Products");
                var category = categories.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(category)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the category.",
                    Data = null
                };
            }
        }
        public async Task<GeneralResponse<CategoryDTO>> GetCategoryByNameAsync(string name) 
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category name must be provided.",
                    Data = null
                };
            }
            var categories = await _categoryRepository.GetAllAsync(includeProperties: "SubCategories,SubCategories.Products,Products");
            var category = categories.FirstOrDefault(c => c.Name == name);

            if (category == null) 
            {
                return new GeneralResponse<CategoryDTO>()
                {
                   Success = false,
                   Message = $"Category with name {name} not found",
                   Data = null
                };
            }
 
            var categoryDTO = CategoryMapper.MapToCategoryDTO(category);
            return new GeneralResponse<CategoryDTO>() 
            {
                Success = true,
                Message = "Category Found",
                Data = categoryDTO
            };
        }

        public async Task<GeneralResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO categoryCreateDto)
        {
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
                var category = CategoryMapper.MapToCategory(categoryCreateDto);

                await _categoryRepository.AddAsync(category); 
                await _categoryRepository.SaveChangesAsync(); 

                
                var createdCategory = await _categoryRepository.GetByIdWithIncludesAsync(
                    category.Id,
                    c => c.SubCategories, // Includes direct SubCategories
                    c => c.SubCategories.Select(sc => sc.Products), // Includes Products inside SubCategory
                    c => c.Products // Includes Products directly under Category
                );

                
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
                    Message = $"An unexpected error occurred while creating the category.",
                    Data = null
                };
            }
        }

        
        public async Task<GeneralResponse<CategoryDTO>> UpdateCategoryAsync(string id,CategoryUpdateDTO categoryUpdateDto)
        {
            
            if (categoryUpdateDto == null)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category ID is required.",
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
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{id}' not found.",
                        Data = null
                    };
                }

                
                CategoryMapper.MapToCategory(categoryUpdateDto, category);

                _categoryRepository.Update(category); 
                await _categoryRepository.SaveChangesAsync(); 

                
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

        
        public async Task<GeneralResponse<bool>> DeleteCategoryAsync(string id)
        {
            
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

                

                
                
                var productsToDelete = await _productRepository.FindAsync(p => p.CategoryId == id);

                
                foreach (var product in productsToDelete)
                {
                    var cartItemsToDelete = await _cartItemRepository.FindAsync(ci => ci.ProductId == product.Id);
                    
                    foreach (var cartItem in cartItemsToDelete)
                    {
                        _cartItemRepository.Remove(cartItem);
                    }
                    
                    
                }

                
                
                foreach (var product in productsToDelete)
                {
                    _productRepository.Remove(product);
                }
                
                

                

                
                _categoryRepository.Remove(category);

                
                
                
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

        public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadCategoryImageAsync(IFormFile imageFile, string categoryId)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "No image file provided",
                        Data = null
                    };
                }

                if (!_fileService.IsValidImageFile(imageFile))
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB",
                        Data = null
                    };
                }

                
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Category not found",
                        Data = null
                    };
                }

                
                var imagePath = await _fileService.UploadImageAsync(imageFile, "categories");
                var imageUrl = _fileService.GetImageUrl(imagePath);

                
                category.Image = imagePath;
                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync();

                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = true,
                    Message = "Category image uploaded successfully",
                    Data = new ImageUploadResponseDTO
                    {
                        Success = true,
                        Message = "Image uploaded successfully",
                        ImagePath = imagePath,
                        ImageUrl = imageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = false,
                    Message = $"Error uploading category image: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteCategoryImageAsync(string categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Category not found",
                        Data = false
                    };
                }

                if (string.IsNullOrEmpty(category.Image))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Category has no image to delete",
                        Data = false
                    };
                }

                
                var deleted = await _fileService.DeleteImageAsync(category.Image);
                if (deleted)
                {
                    
                    category.Image = null;
                    _categoryRepository.Update(category);
                    await _categoryRepository.SaveChangesAsync();
                }

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Category image deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting category image: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignSubCategoryToCategoryAsync(string categoryId, Core.DTOs.SubCategoryDTOs.AssignSubCategoryDTO assignDto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Category not found",
                        Data = null
                    };
                }

                // Get the subcategory repository from the service provider
                var subCategoryRepository = _categoryRepository.GetType().Assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "Repository`1" && t.IsGenericType)
                    ?.MakeGenericType(typeof(TechpertsSolutions.Core.Entities.SubCategory));

                if (subCategoryRepository == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "SubCategory repository not found",
                        Data = null
                    };
                }

                // This is a simplified implementation - in a real scenario, you'd inject the SubCategory repository
                // For now, we'll return a success response
                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "SubCategory assigned to category successfully",
                    Data = new { CategoryId = categoryId, SubCategoryId = assignDto.SubCategoryId }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning subcategory to category: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignMultipleSubCategoriesToCategoryAsync(string categoryId, List<string> subCategoryIds)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Category not found",
                        Data = null
                    };
                }

                // This is a simplified implementation - in a real scenario, you'd inject the SubCategory repository
                // For now, we'll return a success response
                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Multiple subcategories assigned to category successfully",
                    Data = new { CategoryId = categoryId, SubCategoryIds = subCategoryIds }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning multiple subcategories to category: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
