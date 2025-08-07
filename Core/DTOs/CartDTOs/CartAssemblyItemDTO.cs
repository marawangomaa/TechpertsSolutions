using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CartDTOs
{
    public class CartAssemblyItemDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string PcAssemblyId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal ProductTotal { get; set; }
        public decimal AssemblyFee { get; set; }

        public bool IsCustomBuild { get; set; } = true;
    }
}
