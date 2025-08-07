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
    public class WishListConfiguration : BaseEntityConfiguration<WishList>
    {
        public override void Configure(EntityTypeBuilder<WishList> builder)
        {
            base.Configure(builder);
            // Deleting a Customer will cascade to delete their WishList.
            builder.HasOne(wl => wl.Customer)
                   .WithOne(c => c.WishList)
                   .HasForeignKey<WishList>(wl => wl.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            // A Customer can only have one WishList.
            builder.HasIndex(wl => wl.CustomerId).IsUnique();

            // Deleting a WishList will cascade to delete all its WishListItems.
            builder.HasMany(wl => wl.WishListItems)
                   .WithOne(wli => wli.WishList)
                   .HasForeignKey(wli => wli.WishListId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
