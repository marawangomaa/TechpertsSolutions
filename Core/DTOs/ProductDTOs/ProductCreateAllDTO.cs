using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductCreateAllDTO
    {
        public ProductCreateDTO product { get; set; } = new ProductCreateDTO();
        public ProductCreateWarSpecDTO WarrantiesSpecs { get; set; } = new ProductCreateWarSpecDTO();
    }
}
