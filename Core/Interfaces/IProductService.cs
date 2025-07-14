using Core.DTOs.Product;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductCardDTO>> GetAllAsync(
             ProductPendingStatus? status = null,
             string? categoryId = null,
             string? nameSearch = null,
             string? sortBy = null,
             bool sortDescending = false
         );

        Task<ProductDTO?> GetByIdAsync(string id);
        Task CreateAsync(ProductCreateDTO dto);
        Task UpdateAsync(string id, ProductUpdateDTO dto);
        Task DeleteAsync(string id);
    }
}
