using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

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

        //public PCAssemblyReadDTO(PCAssembly pCAssembly)
        //{
        //    Id = pCAssembly.Id;
        //    Name = pCAssembly.Name;
        //    CreatedAt = pCAssembly.CreatedAt;
        //    CustomerId = pCAssembly.CustomerId;
        //    ServiceUsageId = pCAssembly.ServiceUsageId;
        //    Items = pCAssembly.PCAssemblyItems.Select(item => new PCAssemblyItemReadDTO(item)).ToList();
        //}
    }
}
