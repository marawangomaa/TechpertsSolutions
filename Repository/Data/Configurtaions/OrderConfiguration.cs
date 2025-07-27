using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Repository.Data.Configurtaions
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict); // Prevent customer deletion if they have orders

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete order items when order is deleted

            builder.HasOne(o => o.OrderHistory)
                   .WithMany(oh => oh.Orders)
                   .HasForeignKey(o => o.OrderHistoryId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null if order history is deleted

            builder.HasOne(o => o.ServiceUsage)
                   .WithMany(su => su.Orders)
                   .HasForeignKey(o => o.ServiceUsageId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null if service usage is deleted

            builder.HasOne(o => o.Cart)
                   .WithOne(c => c.Order)
                   .HasForeignKey<Order>(o => o.CartId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cart deletion if it has an order

            builder.HasOne(o => o.Delivery)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null if delivery is deleted
        }
    }
}