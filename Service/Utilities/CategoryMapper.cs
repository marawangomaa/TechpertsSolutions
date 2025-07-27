using Core.DTOs.CategoryDTOs;
using Core.DTOs.ProductDTOs;
using Core.DTOs.SubCategoryDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class CategoryMapper
    {
        public static CategoryDTO? MapToCategoryDTO(Category? category)
        {
            if (category == null) return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                Description = category.Description,
                products = category.Products?.Select(p => ProductMapper.MapToProductListItem(p)).Where(p => p != null).ToList() ?? new List<ProductListItemDTO>(),
                SubCategories = category.SubCategories?.Select(sc => SubCategoryMapper.MapToSubCategoryDTO(sc)).Where(sc => sc != null).ToList() ?? new List<SubCategoryDTO>()
            };
        }

        public static Category? MapToCategory(CategoryCreateDTO? dto)
        {
            if (dto == null) return null;

            return new Category
            {
                Name = dto.Name
            };
        }

        public static Category? MapToCategory(CategoryUpdateDTO? dto, Category? existingCategory)
        {
            if (dto == null || existingCategory == null) return null;

            existingCategory.Name = dto.Name;
            return existingCategory;
        }

        public static IEnumerable<CategoryDTO> MapToCategoryDTOList(IEnumerable<Category>? categories)
        {
            if (categories == null) return Enumerable.Empty<CategoryDTO>();

            return categories.Select(MapToCategoryDTO).Where(dto => dto != null)!;
        }
    }
} 