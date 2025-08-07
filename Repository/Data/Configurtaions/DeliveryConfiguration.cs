using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class DeliveryConfiguration : BaseEntityConfiguration<Delivery>
    {
        public override void Configure(EntityTypeBuilder<Delivery> builder)
        {
            base.Configure(builder);
            // Many-to-many with TechCompanies is configured on the other side.
            builder.HasMany(d => d.TechCompanies).WithMany(t => t.Deliveries);

            // Deleting a Customer is restricted if they have a delivery associated with them.
            builder.HasOne(d => d.Customer)
                   .WithMany()
                   .HasForeignKey(d => d.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a DeliveryPerson is restricted if they are assigned to a delivery.
            builder.HasOne(d => d.DeliveryPerson)
                   .WithMany(dp => dp.Deliveries)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting an Order is restricted if it has a delivery.
            builder.HasOne(d => d.Order)
                   .WithOne(o => o.Delivery)
                   .HasForeignKey<Delivery>(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
