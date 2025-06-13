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
    public class StockControlManagerConfiguration : IEntityTypeConfiguration<StockControlManager>
    {
        public void Configure(EntityTypeBuilder<StockControlManager> builder)
        {
            builder.HasOne(s => s.User)
                 .WithOne(u => u.StockControlManager)
                 .HasForeignKey<StockControlManager>(s => s.Id)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
