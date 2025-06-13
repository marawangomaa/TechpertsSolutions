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
    public class SalesManagerConfiguration : IEntityTypeConfiguration<SalesManager>
    {
        public void Configure(EntityTypeBuilder<SalesManager> builder)
        {
            builder.HasOne(s => s.User)
                  .WithOne(u => u.SalesManager)
                  .HasForeignKey<SalesManager>(s => s.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
