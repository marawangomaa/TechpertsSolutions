using Core.Entities;
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
    public class DeliveryAssignmentSettingsConfiguration
        : IEntityTypeConfiguration<DeliveryAssignmentSettings>
    {
        public void Configure(EntityTypeBuilder<DeliveryAssignmentSettings> builder)
        {
            builder.HasNoKey();
        }
    }
}
