using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.ProductDTOs
{
    public class WarrantyDTO
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        //public WarrantyDTO(Warranty warranty)
        //{
        //    Id = warranty.Id;
        //    Type = warranty.Type;
        //    Duration = warranty.Duration;
        //    Description = warranty.Description;
        //    StartDate = warranty.StartDate;
        //    EndDate = warranty.EndDate;
        //}
    }
}
