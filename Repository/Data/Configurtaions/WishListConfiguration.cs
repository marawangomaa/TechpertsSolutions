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
            builder.HasOne(wl => wl.Customer)
                   .WithOne(c => c.WishList)
                   .HasForeignKey<WishList>(wl => wl.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(wl => wl.CustomerId)
                   .IsUnique();

            builder.HasMany(wl => wl.WishListItems)
                   .WithOne(wli => wli.WishList)
                   .HasForeignKey(wli => wli.WishListId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
