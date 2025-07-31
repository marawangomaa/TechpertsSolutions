using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Repository.Data.Configurtaions
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            // Ignore CreatedAt and UpdatedAt properties since they're not mapped to database
            builder.Ignore(e => e.CreatedAt);
            builder.Ignore(e => e.UpdatedAt);
        }
    }
} 
