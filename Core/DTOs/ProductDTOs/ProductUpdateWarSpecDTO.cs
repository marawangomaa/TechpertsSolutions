using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductUpdateWarSpecDTO
    {
        public List<SpecificationCreateDTO>? Specifications { get; set; }
        public List<WarrantyCreateDTO>? Warranties { get; set; }
    }
}
