using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyUpdateDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Budget { get; set; }

        public PCAssemblyStatus? Status { get; set; }

        public string? ServiceUsageId { get; set; }

        public List<PCAssemblyItemUpdateDTO>? Items { get; set; } = new List<PCAssemblyItemUpdateDTO>();
    }
}
