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

            builder.HasOne(wi => wi.WishList)
                   .WithMany(w => w.WishListItems)
                   .HasForeignKey(wi => wi.WishListId)
                   .IsRequired();

            builder.HasOne(wi => wi.Product)
                   .WithMany(p => p.WishListItems)
                   .HasForeignKey(wi => wi.ProductId)
                   .IsRequired();

            builder.HasOne(wi => wi.Cart)
                   .WithMany(c => c.WishListItems)
                   .HasForeignKey(wi => wi.CartId)
                   .IsRequired();
        }
    }
}
