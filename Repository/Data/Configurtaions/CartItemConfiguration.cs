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

            builder.Property(ci => ci.ProductId)
                    .IsRequired(false); //  Make it optional

            builder.HasOne(ci => ci.PCAssembly)
                   .WithMany()
                   .HasForeignKey(ci => ci.PcAssemblyId)
                   .OnDelete(DeleteBehavior.Restrict); // Or ClientSetNull

            builder.Property(ci => ci.PcAssemblyId)
                   .IsRequired(false); // Also optional
        }
    }
}
