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
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class DeliveryClusterTrackingConfig : BaseEntityConfiguration<DeliveryClusterTracking>
    {
        public override void Configure(EntityTypeBuilder<DeliveryClusterTracking> builder)
        {
            base.Configure(builder);

            builder.Property(d => d.Status)
                   .HasDefaultValue(DeliveryClusterStatus.Pending) 
                   .IsRequired();

            builder.Property(d => d.LastRetryTime)
                   .IsRequired(false);

            builder.Property(d => d.clusterId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(d => d.Location)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(d => d.Driver)
                   .HasMaxLength(100);
        }
    }
}
