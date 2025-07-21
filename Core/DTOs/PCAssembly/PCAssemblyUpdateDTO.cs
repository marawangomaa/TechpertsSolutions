using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssembly
{
    public class PCAssemblyUpdateDTO
    {
        public string? Name { get; set; }

        public string? ServiceUsageId { get; set; }

        public List<PCAssemblyItemUpdateDTO>? Items { get; set; } = new List<PCAssemblyItemUpdateDTO>();
    }
}
