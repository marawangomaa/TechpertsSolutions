using Core.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data
{
    public class TechpertsContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public TechpertsContext(DbContextOptions<TechpertsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for CategorySubCategory
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

            // Apply default value for CreatedAt for all entities implementing IEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (typeof(IEntity).IsAssignableFrom(clrType) && clrType != typeof(CategorySubCategory))
                {
                    var entity = modelBuilder.Entity(clrType);

                    entity.Property("CreatedAt")
                          .HasDefaultValueSql("GETUTCDATE()");
                }
            }

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