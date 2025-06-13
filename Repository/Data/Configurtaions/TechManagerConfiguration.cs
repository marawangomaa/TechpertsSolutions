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
    public class TechManagerConfiguration : IEntityTypeConfiguration<TechManager>
    {
        public void Configure(EntityTypeBuilder<TechManager> builder)
        {
            builder.HasOne(t => t.User)
                  .WithOne(u => u.TechManager)
                  .HasForeignKey<TechManager>(t => t.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
