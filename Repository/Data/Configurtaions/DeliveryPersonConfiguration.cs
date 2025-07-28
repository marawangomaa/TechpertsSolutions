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
    public class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
    {
        public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
        {
            builder.HasOne(dp => dp.User)
                 .WithOne(u => u.DeliveryPerson)
                 .HasForeignKey<DeliveryPerson>(dp => dp.UserId)
                 .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(dp => dp.Role)
                 .WithMany()
                 .HasForeignKey(dp => dp.RoleId)
                 .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(dp => dp.Deliveries)
                 .WithOne(d => d.DeliveryPerson)
                 .HasForeignKey(d => d.DeliveryPersonId)
                 .OnDelete(DeleteBehavior.SetNull); 
        }
    }
} 
