using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.WarrantyDTOs
{
    public class WarrantyUpdateDTO
    {
        public string? Type { get; set; }
        public string? Duration { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ProductId { get; set; }
    }
}
