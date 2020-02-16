using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Employ
{
    public class Employ : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Type { get; set; }
        public int UserId { get; set; }

        public User.User User { get; set; }
    }

    public class EmployConfiguration : IEntityTypeConfiguration<Employ>
    {
        public void Configure(EntityTypeBuilder<Employ> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Time).IsRequired();
            builder.Property(p => p.Text).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(c => c.Employs)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.UserId).HasName("IX_Employ_UserId");
        }
    }
}
