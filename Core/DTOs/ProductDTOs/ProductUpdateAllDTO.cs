using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductUpdateAllDTO
    {
        public ProductUpdateDTO product { get; set; }
        public ProductUpdateWarSpecDTO WarrantiesSpecs { get; set; }
    }
}
