using Core.Entities;
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
    public class DeliveryPersonConfig : BaseEntityConfiguration<DeliveryPerson>
    {
        public override void Configure(EntityTypeBuilder<DeliveryPerson> builder)
        {
            base.Configure(builder);

            // User
            builder.HasOne(dp => dp.User)
                   .WithOne(u => u.DeliveryPerson)
                   .HasForeignKey<DeliveryPerson>(dp => dp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Role
            builder.HasOne(dp => dp.Role)
                   .WithMany()
                   .HasForeignKey(dp => dp.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deliveries assigned
            builder.HasMany(dp => dp.Deliveries)
                   .WithOne(d => d.DeliveryPerson)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.SetNull);

            // DeliveryOffers assigned
            builder.HasMany(dp => dp.Offers)
                   .WithOne(o => o.DeliveryPerson)
                   .HasForeignKey(o => o.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(dp => dp.RoleId);
            builder.HasIndex(dp => dp.UserId).IsUnique();
        }
    }
}
