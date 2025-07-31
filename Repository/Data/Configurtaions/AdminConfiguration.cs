using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

namespace TechpertsSolutions.Repository.Data.Configurations
{
    public class AdminConfiguration : BaseEntityConfiguration<Admin>
    {
        public override void Configure(EntityTypeBuilder<Admin> builder)
        {
            base.Configure(builder);
            builder.HasOne(a => a.User)
                   .WithOne(u => u.Admin)
                   .HasForeignKey<Admin>(a => a.Id)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(a => a.Role)
                   .WithMany()
                   .HasForeignKey(a => a.RoleId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
