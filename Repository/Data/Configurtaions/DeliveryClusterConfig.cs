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
    public class DeliveryClusterConfig : BaseEntityConfiguration<DeliveryCluster>
    {
        public override void Configure(EntityTypeBuilder<DeliveryCluster> builder)
        {
            base.Configure(builder);

            // Delivery relationship
            builder.HasOne(dc => dc.Delivery)
                   .WithMany(d => d.Clusters)
                   .HasForeignKey(dc => dc.DeliveryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // TechCompany relationship
            builder.HasOne(dc => dc.TechCompany)
                   .WithMany()
                   .HasForeignKey(dc => dc.TechCompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // AssignedDriver relationship
            builder.HasOne(dc => dc.AssignedDriver)
                   .WithMany()
                   .HasForeignKey(dc => dc.AssignedDriverId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(dc => dc.Status);
            builder.HasIndex(dc => dc.AssignedDriverId);
            builder.HasIndex(dc => dc.DeliveryId);

            // Coordinates precision
            builder.Property(dc => dc.DropoffLatitude).HasColumnType("decimal(9,6)").IsRequired(false);
            builder.Property(dc => dc.DropoffLongitude).HasColumnType("decimal(9,6)").IsRequired(false);
        }
    }
}
