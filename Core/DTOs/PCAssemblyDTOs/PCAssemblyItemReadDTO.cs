using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyItemReadDTO
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
    }
}
