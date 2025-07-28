using Core.Entities;
using Core.Enums;
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
                  .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(o => o.OrderHistory)
                   .WithMany(oh => oh.Orders)
                   .HasForeignKey(o => o.OrderHistoryId)
                   .OnDelete(DeleteBehavior.SetNull); 

            builder.HasOne(o => o.ServiceUsage)
                   .WithMany(su => su.Orders)
                   .HasForeignKey(o => o.ServiceUsageId)
                   .OnDelete(DeleteBehavior.SetNull); 

            builder.HasOne(o => o.Cart)
                   .WithOne(c => c.Order)
                   .HasForeignKey<Order>(o => o.CartId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(o => o.Delivery)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryId)
                   .OnDelete(DeleteBehavior.SetNull); 

            
            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);
        }
    }
}
