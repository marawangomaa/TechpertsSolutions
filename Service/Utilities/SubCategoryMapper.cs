using Core.DTOs.ProductDTOs;
using Core.DTOs.SubCategoryDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class SubCategoryMapper
    {
        public static SubCategoryDTO MapToSubCategoryDTO(SubCategory subCategory)
        {
            if (subCategory == null)
                return null;

            // Get category information from the many-to-many relationship
            var categoryInfo = subCategory.CategorySubCategories?.FirstOrDefault()?.Category;
            var categoryId = categoryInfo?.Id ?? string.Empty;
            var categoryName = categoryInfo?.Name ?? string.Empty;

            // Determine image: SubCategory's image or fallback to Category's (which has a default via getter)
            var finalImage = !string.IsNullOrEmpty(subCategory.Image)
                ? subCategory.Image
                : categoryInfo?.Image ?? "assets/subcategories/default-subcategory.png";

            return new SubCategoryDTO
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                Image = finalImage,
                CategoryId = categoryId,
                CategoryName = categoryName,
                Products =
                    subCategory
                        .Products?.Select(p => ProductMapper.MapToProductCard(p))
                        .Where(p => p != null)
                        .ToList() ?? new List<ProductCardDTO>(),
            };
        }

        public static SubCategory MapToSubCategory(CreateSubCategoryDTO dto)
        {
            if (dto == null)
                return null;

            return new SubCategory { Id = Guid.NewGuid().ToString(), Name = dto.Name };
        }

        public static SubCategory MapToSubCategory(
            UpdateSubCategoryDTO dto,
            SubCategory existingSubCategory
        )
        {
            if (dto == null || existingSubCategory == null)
                return null;

            existingSubCategory.Name = dto.Name;
            return existingSubCategory;
        }

        public static IEnumerable<SubCategoryDTO> MapToSubCategoryDTOList(
            IEnumerable<SubCategory> subCategories
        )
        {
            if (subCategories == null)
                return Enumerable.Empty<SubCategoryDTO>();

            return subCategories.Select(MapToSubCategoryDTO).Where(dto => dto != null);
        }
    }
}
