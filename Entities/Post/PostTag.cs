using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class PostTag : BaseEntity
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }

    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder.Property(p => p.PostId).IsRequired();
            builder.Property(p => p.TagId).IsRequired();

            builder.HasOne(p => p.Post).WithMany(c => c.PostTags).HasForeignKey(p => p.PostId);
            builder.HasOne(p => p.Tag).WithMany(c => c.PostTags).HasForeignKey(p => p.TagId);
        }
    }
}