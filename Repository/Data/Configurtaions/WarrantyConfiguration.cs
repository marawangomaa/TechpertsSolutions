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
    public class WarrantyConfiguration : BaseEntityConfiguration<Warranty>
    {
        public override void Configure(EntityTypeBuilder<Warranty> builder)
        {
            base.Configure(builder);
            builder.HasOne(w => w.Product)
                   .WithMany(p => p.Warranties)
                   .HasForeignKey(w => w.ProductId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
