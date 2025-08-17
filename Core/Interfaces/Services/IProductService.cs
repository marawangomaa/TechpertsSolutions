using Core.DTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Http;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            ProductCategory? categoryEnum = null,
            string? subCategoryName = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false);
        Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetAllTechCompanyProductAsync(
                                                                   int pageNumber = 1,
                                                                   int pageSize = 10,
                                                   ProductPendingStatus? status = null,
                                                   ProductCategory? categoryEnum = null,
                                                           string? subCategoryName = null,
                                                                string? nameSearch = null,
                                                                   string? sortBy = null,
                                                              bool sortDescending = false,
                                                             string? techCompanyId = null);

        Task<GeneralResponse<ProductDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<ProductDTO>> AddAsync(ProductCreateDTO dto,ProductCreateWarSpecDTO warSpecDTO,ProductCategory category, ProductPendingStatus status);
        Task<GeneralResponse<ProductDTO>> UpdateAsync(string id,ProductUpdateDTO dto,ProductUpdateWarSpecDTO warSpecDto,ProductCategory category,ProductPendingStatus status);
        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> ApproveProductAsync(string productId);
        Task<GeneralResponse<bool>> RejectProductAsync(string productId, string reason);
        Task<GeneralResponse<PaginatedDTO<ProductDTO>>> GetPendingProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDescending = false);
        
        Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetProductsByCategoryAsync(
            ProductCategory categoryEnum,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDescending = false);

        Task<GeneralResponse<ImageUploadResponseDTO>> UploadProductImageAsync(ProductCreateImageUploadDTO imageUploadDto, string productId);
        Task<GeneralResponse<bool>> DeleteProductImageAsync(string productId);
    }
}
