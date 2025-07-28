using Core.DTOs.SubCategoryDTOs;
using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class SubCategoryMapper
    {
        public static SubCategoryDTO MapToSubCategoryDTO(SubCategory subCategory)
        {
            if (subCategory == null) return null;

            return new SubCategoryDTO
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                Image = subCategory.Image,
                CategoryId = subCategory.CategoryId,
                CategoryName = subCategory.Category?.Name,
                Products = subCategory.Products?.Select(p => ProductMapper.MapToProductListItem(p)).Where(p => p != null).ToList() ?? new List<ProductListItemDTO>()
            };
        }

        public static SubCategory MapToSubCategory(CreateSubCategoryDTO dto)
        {
            if (dto == null) return null;

            return new SubCategory
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                CategoryId = dto.CategoryId
            };
        }

        public static SubCategory MapToSubCategory(UpdateSubCategoryDTO dto, SubCategory existingSubCategory)
        {
            if (dto == null || existingSubCategory == null) return null;

            existingSubCategory.Name = dto.Name;
            existingSubCategory.CategoryId = dto.CategoryId;
            return existingSubCategory;
        }

        public static IEnumerable<SubCategoryDTO> MapToSubCategoryDTOList(IEnumerable<SubCategory> subCategories)
        {
            if (subCategories == null) return Enumerable.Empty<SubCategoryDTO>();

            return subCategories.Select(MapToSubCategoryDTO).Where(dto => dto != null);
        }
    }
} 