using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class Favorite : BaseEntity
    {
        public int PostId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public Post.Post Post { get; set; }
    }

    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.Property(p => p.PostId).IsRequired();
            builder.Property(p => p.UserId).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(c => c.Favorites)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Post)
                .WithMany(c => c.Favorites)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.UserId).HasName("IX_Favorite_UserId");
        }
    }
}
