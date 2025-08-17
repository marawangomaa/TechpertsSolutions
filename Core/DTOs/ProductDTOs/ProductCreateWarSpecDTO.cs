using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductCreateWarSpecDTO
    {
        public List<SpecificationCreateDTO>? Specifications { get; set; } = new();
        public List<WarrantyCreateDTO>? Warranties { get; set; } = new();
    }
}
