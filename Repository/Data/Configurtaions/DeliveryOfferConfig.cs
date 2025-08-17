using Core.Entities;
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
    public class DeliveryOfferConfig : BaseEntityConfiguration<DeliveryOffer>
    {
        public override void Configure(EntityTypeBuilder<DeliveryOffer> builder)
        {
            base.Configure(builder);

            // Delivery relationship
            builder.HasOne(o => o.Delivery)
                   .WithMany(d => d.Offers)
                   .HasForeignKey(o => o.DeliveryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Optional Cluster relationship
            builder.HasOne(o => o.Cluster)
                   .WithMany()
                   .HasForeignKey(o => o.ClusterId)
                   .OnDelete(DeleteBehavior.SetNull);

            // DeliveryPerson relationship
            builder.HasOne(o => o.DeliveryPerson)
                   .WithMany(dp => dp.Offers)
                   .HasForeignKey(o => o.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(o => o.ExpiryTime);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.DeliveryId);
            builder.HasIndex(o => o.ClusterId);

            // Properties precision
            builder.Property(o => o.OfferedPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
