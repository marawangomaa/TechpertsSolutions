using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.WarrantyDTOs
{
    public class WarrantyCreateDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProductId { get; set; }
    }
}
