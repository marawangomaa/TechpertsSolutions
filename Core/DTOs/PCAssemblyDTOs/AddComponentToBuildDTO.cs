using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class AddComponentToBuildDTO
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        [Required]
        public ProductCategory Category  { get; set; }
    }
}
