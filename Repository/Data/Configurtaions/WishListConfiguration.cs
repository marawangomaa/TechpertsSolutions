using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configurtaions
{
    public class WishListConfiguration : IEntityTypeConfiguration<WishList>
    {
        public void Configure(EntityTypeBuilder<WishList> builder)
        {
            builder.HasOne(wl => wl.Customer)
                   .WithOne(c => c.WishList)
                   .HasForeignKey<WishList>(wl => wl.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete wishlist when customer is deleted

            // Ensure each customer can only have one wishlist
            builder.HasIndex(wl => wl.CustomerId)
                   .IsUnique();

            builder.HasMany(wl => wl.WishListItems)
                   .WithOne(wli => wli.WishList)
                   .HasForeignKey(wli => wli.WishListId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete wishlist items when wishlist is deleted
        }
    }
}
