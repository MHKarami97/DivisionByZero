using Entities.Common;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        }
    }
}