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

            builder.HasOne(d => d.Order)
                   .WithMany(o => o.Deliveries)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Customer)
                   .WithMany(c => c.Deliveries)
                   .HasForeignKey(d => d.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.DeliveryPerson)
                   .WithMany(dp => dp.Deliveries)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.ParentDelivery)
                   .WithMany(p => p.SubDeliveries)
                   .HasForeignKey(d => d.ParentDeliveryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Offers)
                   .WithOne(o => o.Delivery)
                   .HasForeignKey(o => o.DeliveryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.TechCompanies)
                   .WithMany(tc => tc.Deliveries)
                   .UsingEntity<Dictionary<string, object>>(
                       "DeliveryTechCompany",
                       j => j.HasOne<TechCompany>()
                             .WithMany()
                             .HasForeignKey("TechCompanyId")
                             .OnDelete(DeleteBehavior.Cascade),
                       j => j.HasOne<Delivery>()
                             .WithMany()
                             .HasForeignKey("DeliveryId")
                             .OnDelete(DeleteBehavior.Cascade),
                       j =>
                       {
                           j.HasKey("DeliveryId", "TechCompanyId");
                           j.ToTable("DeliveryTechCompanies");
                       });

            builder.Property(d => d.TrackingNumber)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(d => d.CustomerPhone)
                   .HasMaxLength(32)
                   .IsRequired(false);

            builder.Property(d => d.CustomerName)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(d => d.PickupAddress)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(d => d.PickupLatitude).IsRequired(false);
            builder.Property(d => d.PickupLongitude).IsRequired(false);
            builder.Property(d => d.DropoffLatitude).IsRequired(false);
            builder.Property(d => d.DropoffLongitude).IsRequired(false);

            builder.Property(d => d.DeliveryFee)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(d => d.RetryCount)
                   .HasDefaultValue(0);

            builder.Property(d => d.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(d => d.SequenceNumber).HasDefaultValue(0);
            builder.Property(d => d.RouteOrder).HasDefaultValue(0);
            builder.Property(d => d.IsFinalLeg).HasDefaultValue(false);

            builder.Property(d => d.UpdatedAt).IsRequired(false);

            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.CreatedAt);
            builder.HasIndex(d => d.CustomerId);
            builder.HasIndex(d => d.DeliveryPersonId);

            builder.ToTable("Deliveries");
        }
    }
 }