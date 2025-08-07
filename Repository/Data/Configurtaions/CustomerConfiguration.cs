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
    public class CustomerConfiguration : BaseEntityConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder);
            // Deleting a User is restricted if they are a Customer.
            builder.HasOne(c => c.User)
               .WithOne(u => u.Customer)
               .HasForeignKey<Customer>(c => c.UserId) // Change this line
               .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Role is restricted if it's assigned to any Customer.
            builder.HasOne(c => c.Role)
                   .WithMany()
                   .HasForeignKey(c => c.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Customer is restricted if they have existing Orders.
            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.Customer)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Customer is restricted if they have existing Maintenance requests.
            builder.HasMany(c => c.Maintenances)
                   .WithOne(m => m.Customer)
                   .HasForeignKey(m => m.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Customer is restricted if they have existing PC Assemblies.
            builder.HasMany(c => c.PCAssembly)
                   .WithOne(pca => pca.Customer)
                   .HasForeignKey(pca => pca.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
