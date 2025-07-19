using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class SpecificationDTO
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Value { get; set; } = string.Empty;
    }
}
