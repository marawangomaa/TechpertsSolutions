using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Specifications
{
    public class CreateSpecificationDTO
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Key { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Required]
        public string Value { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Required]
        public string ProductId { get; set; } = string.Empty;
    }
}
