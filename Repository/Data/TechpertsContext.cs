using Core.Entities;
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
            // Configure the primary key for AppUser
            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.SenderUser)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(pm => pm.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.ReceiverUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(pm => pm.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StripeSettings>().HasNoKey();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (typeof(IEntity).IsAssignableFrom(clrType))
                {
                    var entity = modelBuilder.Entity(clrType);

                    if (entityType.FindProperty("CreatedAt") != null)
                    {
                        entity.Property("CreatedAt").HasDefaultValueSql("GETDATE()");
                    }

                    if (entityType.FindProperty("UpdatedAt") != null)
                    {
                        entity.Property("UpdatedAt").HasDefaultValueSql("GETDATE()");
                    }
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TechpertsContext).Assembly);
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<DeliveryOffer> DeliveryOffer { get; set; }
        public DbSet<DeliveryCluster> DeliveryCluster { get; set; }
        public DbSet<DeliveryClusterTracking> DeliveryClusterTracking { get; set; }
        public DbSet<DeliveryAssignmentSettings> DeliveryAssignmentSettings { get; set; }
        public DbSet<DeliveryClusterDriverOffer> DeliveryClusterDriverOffer { get; set; }
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
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<StripeSettings> StripeSettings { get; set; }

    }
}