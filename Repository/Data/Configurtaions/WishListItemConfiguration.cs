using Core.Entities;
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
    public class WishListItemConfiguration : IEntityTypeConfiguration<WishListItem>
    {
        public void Configure(EntityTypeBuilder<WishListItem> builder)
        {
            builder.HasOne(wli => wli.WishList)
                   .WithMany(wl => wl.WishListItems)
                   .HasForeignKey(wli => wli.WishListId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Delete wishlist item when wishlist is deleted

            builder.HasOne(wli => wli.Product)
                   .WithMany(p => p.WishListItems)
                   .HasForeignKey(wli => wli.ProductId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if it's in wishlist items
        }
    }
}
