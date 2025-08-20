using Core.DTOs;
using Core.DTOs.SubCategoryDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategorySubCategory> _categorySubCategoryRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<SubCategoryService> _logger;

        public SubCategoryService(
            IRepository<SubCategory> subCategoryRepository,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<CategorySubCategory> categorySubCategoryRepository,
            IFileService fileService,
            ILogger<SubCategoryService> logger
        )
        {
            _subCategoryRepository = subCategoryRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _categorySubCategoryRepository = categorySubCategoryRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetAllSubCategoriesAsync()
        {
            try
            {
                // Use basic includes first
                var subCategories = await _subCategoryRepository.GetAllWithIncludesAsync(
                    sc => sc.Products,
                    sc => sc.CategorySubCategories
                );

                // Manually load the nested properties for each subcategory
                foreach (var subCategory in subCategories)
                {
                    if (subCategory.Products != null)
                    {
                        foreach (var product in subCategory.Products)
                        {
                            // Load specifications and warranties for each product
                            var productWithIncludes =
                                await _productRepository.GetByIdWithIncludesAsync(
                                    product.Id,
                                    p => p.Specifications,
                                    p => p.Warranties,
                                    p => p.Category,
                                    p => p.SubCategory,
                                    p => p.TechCompany, // Add this line
                                    p => p.TechCompany.User
                                );

                            // Update the product with loaded data
                            if (productWithIncludes != null)
                            {
                                product.Specifications = productWithIncludes.Specifications;
                                product.Warranties = productWithIncludes.Warranties;
                                product.Category = productWithIncludes.Category;
                                product.SubCategory = productWithIncludes.SubCategory;
                                product.TechCompany = productWithIncludes.TechCompany;
                            }
                        }
                    }

                    // Manually load the Category navigation property in CategorySubCategories
                    if (subCategory.CategorySubCategories != null)
                    {
                        foreach (var categorySubCategory in subCategory.CategorySubCategories)
                        {
                            var category = await _categoryRepository.GetByIdAsync(
                                categorySubCategory.CategoryId
                            );
                            if (category != null)
                            {
                                categorySubCategory.Category = category;
                            }
                        }
                    }

                    // Temporary: If subcategory is not assigned to any category, assign it to "Processor" category
                    if (
                        subCategory.CategorySubCategories == null
                        || !subCategory.CategorySubCategories.Any()
                    )
                    {
                        var processorCategory = await _categoryRepository.GetFirstOrDefaultAsync(
                            c => c.Name == "Processor"
                        );
                        if (processorCategory != null)
                        {
                            var categorySubCategory = new CategorySubCategory
                            {
                                CategoryId = processorCategory.Id,
                                SubCategoryId = subCategory.Id,
                                AssignedAt = DateTime.Now,
                            };
                            await _categorySubCategoryRepository.AddAsync(categorySubCategory);
                            await _categorySubCategoryRepository.SaveChangesAsync();

                            // Reload the subcategory with the new relationship
                            subCategory.CategorySubCategories = new List<CategorySubCategory>
                            {
                                categorySubCategory,
                            };
                            categorySubCategory.Category = processorCategory;
                        }
                    }
                }

                var subCategoryDtos = subCategories
                    .Select(SubCategoryMapper.MapToSubCategoryDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = subCategoryDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subcategories");
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving subcategories.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                // Use basic includes first
                var subCategory = await _subCategoryRepository.GetFirstOrDefaultWithIncludesAsync(
                    sc => sc.Id == id,
                    sc => sc.Products,
                    sc => sc.CategorySubCategories
                );

                if (subCategory == null)
                {
                    return new GeneralResponse<SubCategoryDTO>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found.",
                        Data = null,
                    };
                }

                // Manually load the nested properties for the subcategory
                if (subCategory.Products != null)
                {
                    foreach (var product in subCategory.Products)
                    {
                        // Load specifications and warranties for each product
                        var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(
                            product.Id,
                            p => p.Specifications,
                            p => p.Warranties,
                            p => p.Category,
                            p => p.SubCategory,
                            p => p.TechCompany,
                            p => p.TechCompany.User
                        );

                        // Update the product with loaded data
                        if (productWithIncludes != null)
                        {
                            product.Specifications = productWithIncludes.Specifications;
                            product.Warranties = productWithIncludes.Warranties;
                            product.Category = productWithIncludes.Category;
                            product.SubCategory = productWithIncludes.SubCategory;
                            product.TechCompany = productWithIncludes.TechCompany;
                        }
                    }
                }

                // Manually load the Category navigation property in CategorySubCategories
                if (subCategory.CategorySubCategories != null)
                {
                    foreach (var categorySubCategory in subCategory.CategorySubCategories)
                    {
                        var category = await _categoryRepository.GetByIdAsync(
                            categorySubCategory.CategoryId
                        );
                        if (category != null)
                        {
                            categorySubCategory.Category = category;
                        }
                    }
                }

                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = true,
                    Message = "SubCategory retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTO(subCategory),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategory by ID: {id}");
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the subcategory.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<SubCategoryDTO>> GetSubCategoryByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory name must be provided.",
                    Data = null,
                };
            }
            var subCategoryByName = await _subCategoryRepository.GetFirstOrDefaultWithIncludesAsync(
                sc => sc.Name == name,
                sc => sc.Products,
                sc => sc.CategorySubCategories
            );
            if (subCategoryByName == null)
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = $"SubCategory Name {name} not found",
                    Data = null,
                };
            }

            // Manually load the nested properties for the subcategory
            if (subCategoryByName.Products != null)
            {
                foreach (var product in subCategoryByName.Products)
                {
                    // Load specifications and warranties for each product
                    var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(
                        product.Id,
                        p => p.Specifications,
                        p => p.Warranties,
                        p => p.Category,
                        p => p.SubCategory,
                        p => p.TechCompany, // Add this line
                        p => p.TechCompany.User
                    );

                    // Update the product with loaded data
                    if (productWithIncludes != null)
                    {
                        product.Specifications = productWithIncludes.Specifications;
                        product.Warranties = productWithIncludes.Warranties;
                        product.Category = productWithIncludes.Category;
                        product.SubCategory = productWithIncludes.SubCategory;
                        product.TechCompany = productWithIncludes.TechCompany;
                    }
                }
            }

            // Manually load the Category navigation property in CategorySubCategories
            if (subCategoryByName.CategorySubCategories != null)
            {
                foreach (var categorySubCategory in subCategoryByName.CategorySubCategories)
                {
                    var category = await _categoryRepository.GetByIdAsync(
                        categorySubCategory.CategoryId
                    );
                    if (category != null)
                    {
                        categorySubCategory.Category = category;
                    }
                }
            }

            var subCategoryDTO = SubCategoryMapper.MapToSubCategoryDTO(subCategoryByName);
            return new GeneralResponse<SubCategoryDTO>
            {
                Success = true,
                Message = $"The SubCategory with name ${name} found successfully",
                Data = subCategoryDTO,
            };
        }

        public async Task<GeneralResponse<SubCategoryDTO>> CreateSubCategoryAsync(
            CreateSubCategoryDTO createDto
        )
        {
            if (createDto == null)
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory data cannot be null.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "SubCategory name is required.",
                    Data = null,
                };
            }

            try
            {
                var existingSubCategory = await _subCategoryRepository.AnyAsync(sc =>
                    sc.Name.ToLower() == createDto.Name.ToLower()
                );
                if (existingSubCategory)
                {
                    return new GeneralResponse<SubCategoryDTO>
                    {
                        Success = false,
                        Message = $"A subcategory with the name '{createDto.Name}' already exists.",
                        Data = null,
                    };
                }

                var subCategory = SubCategoryMapper.MapToSubCategory(createDto);

                await _subCategoryRepository.AddAsync(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                // Get the created subcategory with includes
                var createdSubCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(
                    subCategory.Id,
                    sc => sc.Products,
                    sc => sc.CategorySubCategories
                );

                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = true,
                    Message = "SubCategory created successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTO(createdSubCategory),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subcategory.");
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the subcategory.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateSubCategoryAsync(
            UpdateSubCategoryDTO updateDto
        )
        {
            if (updateDto == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = false,
                };
            }

            if (string.IsNullOrWhiteSpace(updateDto.Id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory ID is required.",
                    Data = false,
                };
            }

            if (!Guid.TryParse(updateDto.Id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = false,
                };
            }

            if (string.IsNullOrWhiteSpace(updateDto.Name))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory name is required.",
                    Data = false,
                };
            }

            // Note: CategoryId is optional in the update DTO since subcategories can be assigned to multiple categories
            // through the CategorySubCategory relationship

            try
            {
                var existingSubCategory = await _subCategoryRepository.GetByIdAsync(updateDto.Id);
                if (existingSubCategory == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{updateDto.Id}' not found.",
                        Data = false,
                    };
                }

                // Validate category exists if CategoryId is provided
                if (!string.IsNullOrWhiteSpace(updateDto.CategoryId))
                {
                    if (!Guid.TryParse(updateDto.CategoryId, out _))
                    {
                        return new GeneralResponse<bool>
                        {
                            Success = false,
                            Message = "Invalid Category ID format. Expected GUID format.",
                            Data = false,
                        };
                    }

                    var categoryExists = await _categoryRepository.AnyAsync(c =>
                        c.Id == updateDto.CategoryId
                    );
                    if (!categoryExists)
                    {
                        return new GeneralResponse<bool>
                        {
                            Success = false,
                            Message = $"Category with ID '{updateDto.CategoryId}' does not exist.",
                            Data = false,
                        };
                    }
                }

                var duplicateExists = await _subCategoryRepository.AnyAsync(sc =>
                    sc.Name.ToLower() == updateDto.Name.ToLower() && sc.Id != updateDto.Id
                );
                if (duplicateExists)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message =
                            $"A subcategory with the name '{updateDto.Name}' already exists in another category.",
                        Data = false,
                    };
                }

                SubCategoryMapper.MapToSubCategory(updateDto, existingSubCategory);

                _subCategoryRepository.Update(existingSubCategory);
                await _subCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "SubCategory updated successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error occurred while updating subcategory with ID: {updateDto.Id}"
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the subcategory.",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteSubCategoryAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "SubCategory ID cannot be null or empty.",
                    Data = false,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = false,
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
                        Data = false,
                    };
                }

                var productsToDelete = await _productRepository.FindAsync(p =>
                    p.SubCategoryId == id
                );
                if (productsToDelete != null && productsToDelete.Any())
                {
                    _productRepository.RemoveRange(productsToDelete.ToList());
                }

                _subCategoryRepository.Remove(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "SubCategory deleted successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting subcategory with ID: {id}");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the subcategory.",
                    Data = false,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<SubCategoryDTO>>
        > GetSubCategoriesByCategoryIdAsync(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Category ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(categoryId, out _))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                    {
                        Success = false,
                        Message = $"Category with ID '{categoryId}' does not exist.",
                        Data = null,
                    };
                }

                // Get subcategories through the CategorySubCategory relationship
                var categorySubCategories = await _categorySubCategoryRepository.FindAsync(csc =>
                    csc.CategoryId == categoryId
                );

                var subCategoryIds = categorySubCategories
                    .Select(csc => csc.SubCategoryId)
                    .ToList();

                var subCategories = await _subCategoryRepository.FindWithStringIncludesAsync(
                    sc => subCategoryIds.Contains(sc.Id),
                    includeProperties: "CategorySubCategories,Products"
                );

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTOList(subCategories),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error occurred while getting subcategories for Category ID: {categoryId}"
                );
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving subcategories.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadSubCategoryImageAsync(
            IFormFile imageFile,
            string subCategoryId
        )
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "No image file provided",
                        Data = null,
                    };
                }

                if (!_fileService.IsValidImageFile(imageFile))
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message =
                            "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB",
                        Data = null,
                    };
                }

                var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
                if (subCategory == null)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "SubCategory not found",
                        Data = null,
                    };
                }

                var imagePath = await _fileService.UploadImageAsync(imageFile, "subcategories");
                var imageUrl = _fileService.GetImageUrl(imagePath);

                subCategory.Image = imagePath;
                _subCategoryRepository.Update(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = true,
                    Message = "SubCategory image uploaded successfully",
                    Data = new ImageUploadResponseDTO
                    {
                        Success = true,
                        Message = "Image uploaded successfully",
                        ImagePath = imagePath,
                        ImageUrl = imageUrl,
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading subcategory image: {ex.Message}");
                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = false,
                    Message = $"Error uploading subcategory image: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteSubCategoryImageAsync(string subCategoryId)
        {
            try
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
                if (subCategory == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "SubCategory not found",
                        Data = false,
                    };
                }

                if (string.IsNullOrEmpty(subCategory.Image))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "SubCategory has no image to delete",
                        Data = false,
                    };
                }

                var deleted = await _fileService.DeleteImageAsync(subCategory.Image);
                if (deleted)
                {
                    subCategory.Image = null;
                    _subCategoryRepository.Update(subCategory);
                    await _subCategoryRepository.SaveChangesAsync();
                }

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "SubCategory image deleted successfully",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subcategory image: {ex.Message}");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting subcategory image: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignSubCategoryToCategoryAsync(
            string subCategoryId,
            string categoryId
        )
        {
            try
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
                if (subCategory == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "SubCategory not found",
                        Data = null,
                    };
                }

                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "Category not found",
                        Data = null,
                    };
                }

                // Check if the relationship already exists
                var existingRelationship =
                    await _categorySubCategoryRepository.GetFirstOrDefaultAsync(csc =>
                        csc.SubCategoryId == subCategoryId && csc.CategoryId == categoryId
                    );

                if (existingRelationship != null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = "SubCategory is already assigned to this category",
                        Data = new { SubCategoryId = subCategoryId, CategoryId = categoryId },
                    };
                }

                // Create the relationship through CategorySubCategory
                var categorySubCategory = new CategorySubCategory
                {
                    CategoryId = categoryId,
                    SubCategoryId = subCategoryId,
                    AssignedAt = DateTime.Now,
                };

                await _categorySubCategoryRepository.AddAsync(categorySubCategory);
                await _categorySubCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "SubCategory assigned to category successfully",
                    Data = new { SubCategoryId = subCategoryId, CategoryId = categoryId },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning subcategory to category: {ex.Message}");
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning subcategory to category: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<object>> AssignSubCategoryToMultipleCategoriesAsync(
            AssignSubCategoryToCategoriesDTO assignDto
        )
        {
            if (assignDto == null)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Assignment data cannot be null.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(assignDto.SubCategoryId))
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "SubCategory ID is required.",
                    Data = null,
                };
            }

            if (assignDto.CategoryIds == null || !assignDto.CategoryIds.Any())
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "At least one Category ID is required.",
                    Data = null,
                };
            }

            try
            {
                // Validate subcategory exists
                var subCategory = await _subCategoryRepository.GetByIdAsync(
                    assignDto.SubCategoryId
                );
                if (subCategory == null)
                {
                    return new GeneralResponse<object>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{assignDto.SubCategoryId}' not found.",
                        Data = null,
                    };
                }

                // Validate all categories exist
                foreach (var categoryId in assignDto.CategoryIds)
                {
                    if (!Guid.TryParse(categoryId, out _))
                    {
                        return new GeneralResponse<object>
                        {
                            Success = false,
                            Message =
                                $"Invalid Category ID format for '{categoryId}'. Expected GUID format.",
                            Data = null,
                        };
                    }

                    var categoryExists = await _categoryRepository.AnyAsync(c =>
                        c.Id == categoryId
                    );
                    if (!categoryExists)
                    {
                        return new GeneralResponse<object>
                        {
                            Success = false,
                            Message = $"Category with ID '{categoryId}' does not exist.",
                            Data = null,
                        };
                    }
                }

                // Create CategorySubCategory relationships
                var categorySubCategories = assignDto
                    .CategoryIds.Select(categoryId => new CategorySubCategory
                    {
                        CategoryId = categoryId,
                        SubCategoryId = assignDto.SubCategoryId,
                        AssignedAt = DateTime.Now,
                    })
                    .ToList();

                // Add all relationships
                foreach (var categorySubCategory in categorySubCategories)
                {
                    await _categorySubCategoryRepository.AddAsync(categorySubCategory);
                }

                await _categorySubCategoryRepository.SaveChangesAsync();

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message =
                        $"SubCategory assigned to {assignDto.CategoryIds.Count} categories successfully",
                    Data = new
                    {
                        SubCategoryId = assignDto.SubCategoryId,
                        CategoryIds = assignDto.CategoryIds,
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error assigning subcategory to multiple categories: {ex.Message}"
                );
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning subcategory to multiple categories: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<SubCategoryDTO>>
        > GetUnassignedSubCategoriesAsync()
        {
            try
            {
                var allSubCategories = await _subCategoryRepository.GetAllAsync(
                    includeProperties: "CategorySubCategories"
                );
                var unassignedSubCategories = allSubCategories
                    .Where(sc =>
                        sc.CategorySubCategories == null || !sc.CategorySubCategories.Any()
                    )
                    .Select(SubCategoryMapper.MapToSubCategoryDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "Unassigned subcategories retrieved successfully",
                    Data = unassignedSubCategories,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unassigned subcategories: {ex.Message}");
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = $"Error getting unassigned subcategories: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<SubCategoryDTO>>
        > GetSubCategoriesWithIncludesAsync(string includeProperties = null)
        {
            try
            {
                var subCategories = await _subCategoryRepository.GetAllAsync(
                    includeProperties: includeProperties
                );
                var subCategoryDtos = subCategories
                    .Select(SubCategoryMapper.MapToSubCategoryDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories with custom includes retrieved successfully.",
                    Data = subCategoryDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error getting subcategories with includes '{includeProperties}': {ex.Message}"
                );
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = $"Error getting subcategories with includes: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<SubCategoryDTO>>
        > GetSubCategoriesByCategoryIdWithIncludesAsync(
            string categoryId,
            string includeProperties = null
        )
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Category ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(categoryId, out _))
            {
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                    {
                        Success = false,
                        Message = $"Category with ID '{categoryId}' does not exist.",
                        Data = null,
                    };
                }

                // Get subcategories through the CategorySubCategory relationship
                var categorySubCategories = await _categorySubCategoryRepository.FindAsync(csc =>
                    csc.CategoryId == categoryId
                );

                var subCategoryIds = categorySubCategories
                    .Select(csc => csc.SubCategoryId)
                    .ToList();

                var subCategories = await _subCategoryRepository.FindWithStringIncludesAsync(
                    sc => subCategoryIds.Contains(sc.Id),
                    includeProperties: includeProperties
                );

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = SubCategoryMapper.MapToSubCategoryDTOList(subCategories),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error occurred while getting subcategories for Category ID: {categoryId} with includes '{includeProperties}'"
                );
                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving subcategories.",
                    Data = null,
                };
            }
        }
    }
}
