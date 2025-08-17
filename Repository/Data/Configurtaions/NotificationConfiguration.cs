using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data.Configurtaions;

public class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Message)
               .IsRequired()
               .HasMaxLength(1000);

        builder.HasOne(n => n.Receiver)
               .WithMany()
               .HasForeignKey(n => n.ReceiverUserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}