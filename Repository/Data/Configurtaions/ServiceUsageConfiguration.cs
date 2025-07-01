using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configurtaions
{
    public class ServiceUsageConfiguration : IEntityTypeConfiguration<ServiceUsage>
    {
        public void Configure(EntityTypeBuilder<ServiceUsage> builder)
        {
            
        }
    }
}
