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
    public class OrderHistoryConfiguration : BaseEntityConfiguration<OrderHistory>
    {
        public override void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            base.Configure(builder);
            // Deleting an OrderHistory will set the foreign key on Orders to null.
            builder.HasMany(oh => oh.Orders)
                   .WithOne(o => o.OrderHistory)
                   .HasForeignKey(o => o.OrderHistoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
