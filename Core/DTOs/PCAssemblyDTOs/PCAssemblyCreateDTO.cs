using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyCreateDTO
    {
        [Required]
        public string CustomerId { get; set; }

        public string? Name { get; set; }

        public string? ServiceUsageId { get; set; }

        public List<PCAssemblyItemCreateDTO>? Items { get; set; } = new List<PCAssemblyItemCreateDTO>();
    }
}
