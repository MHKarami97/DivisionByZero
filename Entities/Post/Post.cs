using System;
using System.Collections.Generic;
using Entities.Common;
using Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Time { get; set; }
        public string ShortDescription { get; set; }
        public int TimeToRead { get; set; }
        public string Image { get; set; }
        public int? Type { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }

        public Category Category { get; set; }
        public User.User User { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<PostTag> PostTags { get; set; }
        public ICollection<View> Views { get; set; }
    }

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.ShortDescription).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Image).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Time).IsRequired();
            builder.Property(p => p.Text).IsRequired();

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.User)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.CategoryId).HasName("IX_Post_CategoryId");
            builder.HasIndex(a => a.UserId).HasName("IX_Post_UserId");
        }
    }
}