using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductCreateImageUploadDTO
    {
        public IFormFile? ImageUrl { get; set; }
        public List<IFormFile>? ImageUrls { get; set; }
    }
}
