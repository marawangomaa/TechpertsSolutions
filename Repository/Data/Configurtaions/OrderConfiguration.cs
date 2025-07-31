using TechpertsSolutions.Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);
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

 

            
            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);
        }
    }
}
