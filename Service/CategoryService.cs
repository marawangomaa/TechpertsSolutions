using Core.DTOs.Category;
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
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllWithIncludesAsync(
                c => c.SubCategories,
                c => c.Products
            );

            // Use CategoryMapper for mapping
            return CategoryMapper.MapToCategoryDTOList(categories);
        }

        // Retrieves a single category by ID, including its sub-categories and products.
        public async Task<CategoryDTO?> GetCategoryByIdAsync(string id)
        {
            var category = await _categoryRepository.GetByIdWithIncludesAsync(
                id,
                c => c.SubCategories,
                c => c.Products
            );

            if (category == null)
            {
                return null;
            }

            // Use CategoryMapper for mapping
            return CategoryMapper.MapToCategoryDTO(category);
        }

        // Creates a new category from a DTO.
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO categoryCreateDto)
        {
            // Use CategoryMapper for mapping
            var category = CategoryMapper.MapToCategory(categoryCreateDto);

            await _categoryRepository.AddAsync(category); // Add entity via repository
            await _categoryRepository.SaveChangesAsync(); // Save changes

            // Use CategoryMapper for mapping
            return CategoryMapper.MapToCategoryDTO(category);
        }

        // Updates an existing category from a DTO.
        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryUpdateDto.Id);
            if (category == null)
            {
                return false; // Category not found
            }

            // Use CategoryMapper for mapping
            CategoryMapper.MapToCategory(categoryUpdateDto, category);

            _categoryRepository.Update(category); // Mark entity as updated
            await _categoryRepository.SaveChangesAsync(); // Save changes
            return true;
        }

        // Deletes a category by ID.
        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false; // Category not found
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
            return true;
        }
    }
}
