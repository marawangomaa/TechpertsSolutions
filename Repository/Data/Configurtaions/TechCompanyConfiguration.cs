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
    class TechCompanyConfiguration : IEntityTypeConfiguration<TechCompany>
    {
        public void Configure(EntityTypeBuilder<TechCompany> builder)
        {
            builder.HasOne(t => t.User)
                    .WithOne(u => u.TechCompany)
                        .HasForeignKey<TechCompany>(u => u.UserId).IsRequired();
        }
    }
}
