using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
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

            builder.HasOne(o => o.SalesManager)
                   .WithMany(s => s.Orders)
                   .HasForeignKey(o => o.SalesManagerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.orderItems)
            .WithOne(oi => oi.Order);

            builder.HasOne(o => o.OrderHistory)
            .WithMany(oh => oh.Orders);

            builder.HasOne(o => o.ServiceUsage)
            .WithMany(su => su.Orders);

            builder.HasOne(o => o._Cart)
            .WithOne(c => c.Order);
        }
    }

}