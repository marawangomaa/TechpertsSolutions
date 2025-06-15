using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data
{
    public class TechpertsContext : DbContext
    {
        public TechpertsContext(DbContextOptions<TechpertsContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PCAssembly> PCAssemblies { get; set; }
        public DbSet<PCAssemblyItem> PCAssemblyItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SalesManager> SalesManagers { get; set; }
        public DbSet<ServiceUsage> ServiceUsages { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<StockControlManager> StockControlManagers { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<TechCompany> TechCompanies { get; set; }
        public DbSet<TechManager> TechManagers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
    }
}
