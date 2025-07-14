using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
<<<<<<< HEAD
        public int Stock {  get; set; }
        public string? ImageUrl { get; set; }
=======
        public int StockQuantity { get; set; }
>>>>>>> f87b149283b60c7ec0dfab29fe049911a598a8c0
        public string CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ImageUrl { get; set; }
        public string? SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
        public ProductPendingStatus status { get; set; } = ProductPendingStatus.Pending;
        public ICollection<Specification>? Specifications { get; set; }
        public ICollection<Warranty>? Warranties { get; set; }

        public string TechManagerId { get; set; }
        public TechManager? TechManager { get; set; }

        public string StockControlManagerId { get; set; }
        public StockControlManager? StockControlManager { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public List<WishListItem>? WishListItems { get; set; }
        public List<PCAssemblyItem>? PCAssemblyItems { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
