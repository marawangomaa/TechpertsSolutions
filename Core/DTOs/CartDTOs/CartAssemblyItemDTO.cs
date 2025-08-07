using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CartDTOs
{
    public class CartAssemblyItemDTO
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsCustomBuild { get; set; }  // <-- Add this flag
        public string PcAssemblyId { get; set; }
    }
}
