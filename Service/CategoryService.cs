using Core.DTOs;
using Core.DTOs.CategoryDTOs;
using Core.DTOs.SubCategoryDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<CategorySubCategory> _categorySubCategoryRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<SubCategory> subCategoryRepository,
            IRepository<CategorySubCategory> categorySubCategoryRepository,
            IFileService fileService,
            ILogger<CategoryService> logger
        )
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
            _subCategoryRepository = subCategoryRepository;
            _categorySubCategoryRepository = categorySubCategoryRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<GeneralResponse<IEnumerable<CategoryDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(q =>
                    q.Include(c => c.Products)
                        .ThenInclude(p => p.Specifications)
                        .Include(c => c.Products)
                        .ThenInclude(p => p.Warranties)
                        .Include(c => c.Products)
                        .ThenInclude(p => p.SubCategory)
                        .Include(c => c.Products)
                        .ThenInclude(p => p.TechCompany)
                        .ThenInclude(tc => tc.User)
                        .Include(c => c.SubCategories)
                        .ThenInclude(cs => cs.SubCategory)
                );

                // Use the centralized mapper for each category.
                var categoryDtos = categories
                    .Select(CategoryMapper.MapToCategoryDTO)
                    .Where(dto => dto != null) // Ensure no nulls are returned
                    .ToList();

                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully.",
                    Data = categoryDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<CategoryDTO>>
                {
                    Success = false,
                    Message =
                        $"An unexpected error occurred while retrieving categories. {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<CategoryDTO>> GetCategoryByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Invalid Category ID format.",
                    Data = null,
                };
            }

            try
            {
                var category = await _categoryRepository.GetFirstOrDefaultAsync(
                    c => c.Id == id,
                    q =>
                        q.Include(c => c.Products)
                            .ThenInclude(p => p.Specifications)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.Warranties)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.SubCategory)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.TechCompany)
                            .ThenInclude(tc => tc.User)
                            .Include(c => c.SubCategories)
                            .ThenInclude(cs => cs.SubCategory)
                );

                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(category),
                };
            }
            catch (Exception ex)
            {
                // It's good practice to log the exception for debugging purposes
                // _logger.LogError(ex, "Error retrieving category by ID: {Id}", id);
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = $"Error retrieving category: {ex.Message}",
                    Data = null,
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
                    Data = null,
                };
            }

            try
            {
                var category = await _categoryRepository.GetFirstOrDefaultAsync(
                    c => c.Name == name,
                    q =>
                        q.Include(c => c.Products)
                            .ThenInclude(p => p.Specifications)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.Warranties)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.SubCategory)
                            .Include(c => c.Products)
                            .ThenInclude(p => p.TechCompany)
                            .ThenInclude(tc => tc.User)
                            .Include(c => c.SubCategories)
                            .ThenInclude(cs => cs.SubCategory)
                );

                if (category == null)
                {
                    return new GeneralResponse<CategoryDTO>
                    {
                        Success = false,
                        Message = $"Category with name '{name}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(category),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = $"Error retrieving category: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Category name is required.",
                    Data = null,
                };
            }

            try
            {
                var category = CategoryMapper.MapToCategory(dto);
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category created successfully.",
                    Data = CategoryMapper.MapToCategoryDTO(category),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<CategoryDTO>> UpdateCategoryAsync(
            string id,
            CategoryUpdateDTO dto
        )
        {
            if (
                dto == null
                || string.IsNullOrWhiteSpace(dto.Name)
                || string.IsNullOrWhiteSpace(id)
                || !Guid.TryParse(id, out _)
            )
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Invalid update data.",
                    Data = null,
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
                        Message = "Category not found.",
                        Data = null,
                    };
                }

                CategoryMapper.MapToCategory(dto, category);
                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync();

                return new GeneralResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category updated.",
                    Data = CategoryMapper.MapToCategoryDTO(category),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteCategoryAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Category ID.",
                    Data = false,
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
                        Message = "Category not found.",
                        Data = false,
                    };
                }

                var products = await _productRepository.FindAsync(p => p.CategoryId == id);
                foreach (var product in products)
                {
                    var cartItems = await _cartItemRepository.FindAsync(ci =>
                        ci.ProductId == product.Id
                    );
                    _cartItemRepository.RemoveRange(cartItems);
                    _productRepository.Remove(product);
                }

                _categoryRepository.Remove(category);
                await _categoryRepository.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Category deleted.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadCategoryImageAsync(
            IFormFile imageFile,
            string categoryId
        )
        {
            try
            {
                if (imageFile == null || !_fileService.IsValidImageFile(imageFile))
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid image file.",
                        Data = null,
                    };
                }

                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Category not found.",
                        Data = null,
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
                    Message = "Image uploaded successfully.",
                    Data = new ImageUploadResponseDTO
                    {
                        Success = true,
                        Message = "Uploaded",
                        ImagePath = imagePath,
                        ImageUrl = imageUrl,
                    },
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = false,
                    Message = $"Error uploading image: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteCategoryImageAsync(string categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null || string.IsNullOrEmpty(category.Image))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Category or image not found.",
                        Data = false,
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
                    Message = "Image deleted successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting image: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignSubCategoryToCategoryAsync(
            string categoryId,
            AssignSubCategoryDTO assignDto
        )
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                var subCategory = await _subCategoryRepository.GetByIdAsync(
                    assignDto.SubCategoryId
                );

                if (category == null || subCategory == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Category or SubCategory not found.",
                        Data = null,
                    };
                }

                var exists = await _categorySubCategoryRepository.AnyAsync(cs =>
                    cs.CategoryId == categoryId && cs.SubCategoryId == assignDto.SubCategoryId
                );

                if (!exists)
                {
                    await _categorySubCategoryRepository.AddAsync(
                        new CategorySubCategory
                        {
                            CategoryId = categoryId,
                            SubCategoryId = assignDto.SubCategoryId,
                        }
                    );
                    await _categorySubCategoryRepository.SaveChangesAsync();
                }

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "SubCategory assigned successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning subcategory: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignMultipleSubCategoriesToCategoryAsync(
            string categoryId,
            AssignSubCategoriesDTO assignDto
        )
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Category not found.",
                        Data = null,
                    };
                }

                foreach (var subCategoryId in assignDto.SubCategoryIds)
                {
                    var exists = await _categorySubCategoryRepository.AnyAsync(cs =>
                        cs.CategoryId == categoryId && cs.SubCategoryId == subCategoryId
                    );

                    if (!exists)
                    {
                        await _categorySubCategoryRepository.AddAsync(
                            new CategorySubCategory
                            {
                                CategoryId = categoryId,
                                SubCategoryId = subCategoryId,
                            }
                        );
                    }
                }

                await _categorySubCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Multiple SubCategories assigned successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning subcategories: {ex.Message}",
                    Data = null,
                };
            }
        }
    }
}
