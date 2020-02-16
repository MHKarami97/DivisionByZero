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
        public DateTimeOffset Time { get; set; }

        public User User { get; set; }
        public Post.Post Post { get; set; }
    }

    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.Property(p => p.PostId).IsRequired();

            builder.HasOne(p => p.User).WithMany(c => c.Likes).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.Post).WithMany(c => c.Likes).HasForeignKey(p => p.PostId);
        }
    }
}