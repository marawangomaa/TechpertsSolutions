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
        private readonly IFileService _fileService;
        private readonly ILogger<SubCategoryService> _logger;

        public SubCategoryService(
            IRepository<SubCategory> subCategoryRepository,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IFileService fileService,
            ILogger<SubCategoryService> logger)
        {
            _subCategoryRepository = subCategoryRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<GeneralResponse<IEnumerable<SubCategoryDTO>>> GetAllSubCategoriesAsync()
        {
            try
            {
                // Optimized includes for subcategory listing with category information
                var subCategories = await _subCategoryRepository.GetAllWithIncludesAsync(
                    sc => sc.Category,
                    sc => sc.Products);

                var subCategoryDtos = subCategories.Select(SubCategoryMapper.MapToSubCategoryDTO).ToList();

                return new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "SubCategories retrieved successfully.",
                    Data = subCategoryDtos
                };
            }
            catch (Exception ex)
            {
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
                // Comprehensive includes for detailed subcategory view with category information
                var subCategory = await _subCategoryRepository.GetByIdWithIncludesAsync(
                    id,
                    sc => sc.Category,
                    sc => sc.Products);

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
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the subcategory.",
                    Data = null
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
                    Data = null
                };
            }
            var subCategoryByName = await _subCategoryRepository.GetFirstOrDefaultAsync(sc => sc.Name == name,
                includeProperties: "Category,Products");
            if (subCategoryByName == null) 
            {
                return new GeneralResponse<SubCategoryDTO>
                {
                    Success = false,
                    Message = $"SubCategory Name {name} not found",
                    Data = null
                };
            }
            var subCategoryDTO = SubCategoryMapper.MapToSubCategoryDTO(subCategoryByName);
            return new GeneralResponse<SubCategoryDTO> 
            {
                Success = true,
                Message = $"The SubCategory with name ${name} found successfully",
                Data = subCategoryDTO
            };
        }
        public async Task<GeneralResponse<SubCategoryDTO>> CreateSubCategoryAsync(CreateSubCategoryDTO createDto)
        {
            
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

                
                var existingSubCategory = await _subCategoryRepository.AnyAsync(
                    sc => sc.Name.ToLower() == createDto.Name.ToLower());
                if (existingSubCategory)
                {
                    return new GeneralResponse<SubCategoryDTO>
                    {
                        Success = false,
                        Message = $"A subcategory with the name '{createDto.Name}' already exists in another category.",
                        Data = null
                    };
                }

                
                var subCategory = SubCategoryMapper.MapToSubCategory(createDto);

                await _subCategoryRepository.AddAsync(subCategory);
                await _subCategoryRepository.SaveChangesAsync();

                
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

                
                var duplicateExists = await _subCategoryRepository.AnyAsync(
                    sc => sc.Name.ToLower() == updateDto.Name.ToLower() &&
                          sc.Id != updateDto.Id);
                if (duplicateExists)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"A subcategory with the name '{updateDto.Name}' already exists in another category.",
                        Data = false
                    };
                }

                
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

                
                
                
                var productsToDelete = await _productRepository.FindAsync(p => p.SubCategoryId == id);
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

        public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadSubCategoryImageAsync(IFormFile imageFile, string subCategoryId)
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

                
                var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
                if (subCategory == null)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "SubCategory not found",
                        Data = null
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
                        ImageUrl = imageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading subcategory image: {ex.Message}");
                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = false,
                    Message = $"Error uploading subcategory image: {ex.Message}",
                    Data = null
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
                        Data = false
                    };
                }

                if (string.IsNullOrEmpty(subCategory.Image))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "SubCategory has no image to delete",
                        Data = false
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
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subcategory image: {ex.Message}");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting subcategory image: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
