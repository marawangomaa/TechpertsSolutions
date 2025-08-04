using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data
{
    public class TechpertsContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public TechpertsContext(DbContextOptions<TechpertsContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly ignore CreatedAt and UpdatedAt properties for all BaseEntity types
            modelBuilder.Entity<Admin>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Cart>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<CartItem>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Category>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Customer>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Delivery>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<DeliveryPerson>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Maintenance>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Order>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<OrderItem>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<PCAssembly>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<PCAssemblyItem>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Product>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<ServiceUsage>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Specification>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<SubCategory>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<TechCompany>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<Warranty>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<WishList>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            modelBuilder.Entity<WishListItem>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore AppUser properties that are not in the database
            modelBuilder.Entity<AppUser>().Ignore(e => e.IsActive).Ignore(e => e.ProfilePhotoUrl);
            
            // Ignore Product image properties that are not in the database
            modelBuilder.Entity<Product>().Ignore(e => e.Image1Url).Ignore(e => e.Image2Url).Ignore(e => e.Image3Url).Ignore(e => e.Image4Url);
            
            // Ignore Notification properties that are not in the database
            modelBuilder.Entity<Notification>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore CommissionPlan properties that are not in the database
            modelBuilder.Entity<CommissionPlan>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore CommissionTransaction properties that are not in the database
            modelBuilder.Entity<CommissionTransaction>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore ChatRoom properties that are not in the database
            modelBuilder.Entity<ChatRoom>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore ChatMessage properties that are not in the database
            modelBuilder.Entity<ChatMessage>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Ignore ChatParticipant properties that are not in the database
            modelBuilder.Entity<ChatParticipant>().Ignore(e => e.CreatedAt).Ignore(e => e.UpdatedAt);
            
            // Explicitly configure CategorySubCategory entity with primary key
            modelBuilder.Entity<CategorySubCategory>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.SubCategoryId });
                
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.SubCategory)
                      .WithMany(sc => sc.CategorySubCategories)
                      .HasForeignKey(e => e.SubCategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(e => e.AssignedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PCAssembly> PCAssemblies { get; set; }
        public DbSet<PCAssemblyItem> PCAssemblyItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ServiceUsage> ServiceUsages { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<CategorySubCategory> CategorySubCategories { get; set; }
        public DbSet<TechCompany> TechCompanies { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CommissionPlan> CommissionPlans { get; set; }
        public DbSet<CommissionTransaction> CommissionTransactions { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
    }
}
