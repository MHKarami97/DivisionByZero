using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class Comment : BaseEntity
    {
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }

        public Post Post { get; set; }
        public User.User User { get; set; }
    }

    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(p => p.Text).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Time).IsRequired();
            builder.Property(p => p.PostId).IsRequired();
            builder.Property(p => p.UserId).IsRequired();

            builder.HasOne(p => p.Post).WithMany(c => c.Comments).HasForeignKey(p => p.PostId);
            builder.HasOne(p => p.User).WithMany(c => c.Comments).HasForeignKey(p => p.UserId);

            builder.HasIndex(a => a.PostId).HasName("IX_Comment_PostId");
        }
    }
}
