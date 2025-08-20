using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class CartItem : BaseEntity
    {
        public string? ProductId { get; set; }
        public string? PcAssemblyId { get; set; }
        public bool IsCustomBuild { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public decimal? ProductTotal { get; set; }
        public decimal? AssemblyFee { get; set; }
        public int Quantity { get; set; }
        public string CartId { get; set; }
        public Cart Cart { get; set; }
        public PCAssembly? PCAssembly { get; set; }

    }
}
