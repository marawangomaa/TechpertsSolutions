using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configurtaions
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasMany(d => d.TechCompanies)
                    .WithMany(t => t.Deliveries);

            builder.HasOne(d => d.Customer)
                   .WithMany()
                   .HasForeignKey(d => d.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent customer deletion if they have deliveries

            builder.HasOne(d => d.DeliveryPerson)
                   .WithMany(dp => dp.Deliveries)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null if delivery person is deleted
        }
    }
}
