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
    public class PrivateMessageConfiguration : IEntityTypeConfiguration<PrivateMessage>
    {
        public void Configure(EntityTypeBuilder<PrivateMessage> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.HasOne(pm => pm.SenderUser)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(pm => pm.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pm => pm.ReceiverUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(pm => pm.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(pm => pm.MessageText)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(pm => pm.SentAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}