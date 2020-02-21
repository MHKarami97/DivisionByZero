using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities.User
{
    public class Like : BaseEntity
    {
        public int PostId { get; set; }
        public int? UserId { get; set; }
        public float Rate { get; set; }
        public DateTimeOffset Time { get; set; }

        public User User { get; set; }
        public Post.Post Post { get; set; }
    }

    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.Property(p => p.PostId).IsRequired();
            builder.Property(p => p.Rate).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(c => c.Likes)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Post)
                .WithMany(c => c.Likes)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.UserId).HasName("IX_Like_UserId");
        }
    }
}