using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyReadDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string? ServiceUsageId { get; set; }
        public List<PCAssemblyItemReadDTO> Items { get; set; } = new List<PCAssemblyItemReadDTO>();
    }
}
