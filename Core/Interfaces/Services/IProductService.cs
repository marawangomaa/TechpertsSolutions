using Core.DTOs.Product;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            string? categoryId = null,
            string? subCategoryId = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false
        );

        Task<GeneralResponse<ProductDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<ProductDTO>> AddAsync(ProductCreateDTO dto);
        Task<GeneralResponse<bool>> UpdateAsync(string id, ProductUpdateDTO dto);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
    }
}
