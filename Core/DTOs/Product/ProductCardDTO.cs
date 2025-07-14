using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class ProductCardDTO
    {
      public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; } 
    public string? CategoryName { get; set; }
    public string Status { get; set; }

    }
}
