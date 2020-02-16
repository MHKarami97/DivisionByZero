using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities.User
{
    public class View : BaseEntity
    {
        public int PostId { get; set; }
        public int? UserId { get; set; }
        public string Ip { get; set; }
        public DateTimeOffset Time { get; set; }

        public User User { get; set; }
        public Post.Post Post { get; set; }
    }

    public class ViewConfiguration : IEntityTypeConfiguration<View>
    {
        public void Configure(EntityTypeBuilder<View> builder)
        {
            builder.Property(p => p.PostId).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(c => c.Views)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Post)
                .WithMany(c => c.Views)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.UserId).HasName("IX_View_UserId");
        }
    }
}