using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class Follower : BaseEntity
    {
        public int FollowerId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
    }

    public class FollowerConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> builder)
        {
            builder.Property(p => p.FollowerId).IsRequired();
            builder.Property(p => p.UserId).IsRequired();

            builder.HasOne(p => p.User).WithMany(c => c.Followers).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.User).WithMany(c => c.Followers).HasForeignKey(p => p.FollowerId);

            builder.HasIndex(a => a.UserId).HasName("IX_Follower_UserId");
        }
    }
}
