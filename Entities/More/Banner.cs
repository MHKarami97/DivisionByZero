using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.More
{
    public class Banner : BaseEntity
    {
        public string Image { get; set; }
        public string Address { get; set; }
        public int Type { get; set; }
    }

    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.Property(p => p.Image).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Type).IsRequired();
        }
    }
}