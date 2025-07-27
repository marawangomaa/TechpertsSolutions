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
    class TechCompanyConfiguration : IEntityTypeConfiguration<TechCompany>
    {
        public void Configure(EntityTypeBuilder<TechCompany> builder)
        {
            builder.HasOne(t => t.User)
                    .WithOne(u => u.TechCompany)
                    .HasForeignKey<TechCompany>(t => t.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade); // Delete tech company when user is deleted

            builder.HasOne(t => t.Role)
                    .WithMany()
                    .HasForeignKey(t => t.RoleId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent role deletion if tech companies use it

            builder.HasMany(t => t.Maintenances)
                    .WithOne(m => m.TechCompany)
                    .HasForeignKey(m => m.TechCompanyId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent tech company deletion if it has maintenances

            builder.HasMany(t => t.Products)
                    .WithOne(p => p.TechCompany)
                    .HasForeignKey(p => p.TechCompanyId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent tech company deletion if it has products

            builder.HasMany(t => t.Deliveries)
                    .WithMany(d => d.TechCompanies);
        }
    }
}
