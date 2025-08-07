using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace Repository.Data.Configurtaions
{
    public class WishListItemConfiguration : BaseEntityConfiguration<WishListItem>
    {
        public override void Configure(EntityTypeBuilder<WishListItem> builder)
        {
            base.Configure(builder);
            // Deleting a WishList will cascade to delete its items.
            builder.HasOne(wli => wli.WishList)
                   .WithMany(wl => wl.WishListItems)
                   .HasForeignKey(wli => wli.WishListId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Deleting a Product is restricted if it's in a WishList.
            builder.HasOne(wli => wli.Product)
                   .WithMany(p => p.WishListItems)
                   .HasForeignKey(wli => wli.ProductId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
