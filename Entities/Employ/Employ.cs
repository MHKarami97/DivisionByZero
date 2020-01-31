using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Employ
{
    public class Employ : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public int AuthorId { get; set; }

        public User.User Author { get; set; }
    }

    public class EmployConfiguration : IEntityTypeConfiguration<Employ>
    {
        public void Configure(EntityTypeBuilder<Employ> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Time).IsRequired();
            builder.Property(p => p.Text).IsRequired();
            builder.HasOne(p => p.Author).WithMany(c => c.Employs).HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
