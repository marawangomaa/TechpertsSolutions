using TechpertsSolutions.Core.Entities;
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
    public class CartItemConfiguration : BaseEntityConfiguration<CartItem>
    {
        public override void Configure(EntityTypeBuilder<CartItem> builder)
        {
            base.Configure(builder);

            // Deleting a Product is restricted if it's in a cart.
            builder.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

            // Deleting a Cart should cascade to delete its items.
            builder.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

            // This makes the ProductId optional for cases where a PC assembly is being referenced instead.
            builder.Property(ci => ci.ProductId)
                    .IsRequired(false);

            // Configure the relationship for PCAssembly using the correct property name.
            builder.HasOne(ci => ci.PCAssembly)
                    .WithMany() // Assuming PCAssembly doesn't have a navigation property back to CartItem
                    .HasForeignKey(ci => ci.PCAssemblyId) // Use the correct property: PCAssemblyId
                    .OnDelete(DeleteBehavior.Restrict);

            // Make the PCAssemblyId optional as not all items are from a PC assembly.
            builder.Property(ci => ci.PCAssemblyId)
                    .IsRequired(false);
        }
    }
}
