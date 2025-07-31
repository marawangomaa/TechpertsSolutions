using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Repository.Data.Configurtaions;
using TechpertsSolutions.Core.Entities;

namespace Repository.Data.Configurtaions
{
    class TechCompanyConfiguration : BaseEntityConfiguration<TechCompany>
    {
        public override void Configure(EntityTypeBuilder<TechCompany> builder)
        {
            base.Configure(builder);
            builder.HasOne(t => t.User)
                    .WithOne(u => u.TechCompany)
                    .HasForeignKey<TechCompany>(t => t.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(t => t.Role)
                    .WithMany()
                    .HasForeignKey(t => t.RoleId)
                    .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(t => t.Maintenances)
                    .WithOne(m => m.TechCompany)
                    .HasForeignKey(m => m.TechCompanyId)
                    .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(t => t.Products)
                    .WithOne(p => p.TechCompany)
                    .HasForeignKey(p => p.TechCompanyId)
                    .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(t => t.Deliveries)
                    .WithMany(d => d.TechCompanies);
        }
    }
}
