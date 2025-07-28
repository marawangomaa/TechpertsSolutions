using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasOne(c => c.User)
                   .WithOne(u => u.Customer)
                   .HasForeignKey<Customer>(c => c.Id)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(c => c.Role)
                   .WithMany()
                   .HasForeignKey(c => c.RoleId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.Customer)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(c => c.Maintenances)
                   .WithOne(m => m.Customer)
                   .HasForeignKey(m => m.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(c => c.PCAssembly)
                   .WithOne(pca => pca.Customer)
                   .HasForeignKey(pca => pca.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
