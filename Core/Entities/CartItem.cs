using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class CartItem : BaseEntity
    {
        // Foreign key to the Product
        public string ProductId { get; set; }

        // This is the correct foreign key to the PCAssembly
        public string? PCAssemblyId { get; set; }

        public int Quantity { get; set; }

        // This will store the base price of the product
        public decimal UnitPrice { get; set; }

        // Foreign key to the Cart
        public string CartId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Cart Cart { get; set; }

        // Navigation property to the PCAssembly
        public PCAssembly? PCAssembly { get; set; }
    }
}