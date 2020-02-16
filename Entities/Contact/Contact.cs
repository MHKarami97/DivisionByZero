using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Entities.Contact
{
    public class Contact : BaseEntity
    {
        public string Text { get; set; }
        public DateTimeOffset Time { get; set; }
        public int UserId { get; set; }
        public int? ParentContactId { get; set; }
        public bool ByServer { get; set; }

        public User.User User { get; set; }
        public Contact ParentContact { get; set; }

        public ICollection<Contact> ChildContacts { get; set; }
    }

    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(p => p.Time).IsRequired();
            builder.Property(p => p.Text).IsRequired();
            builder.Property(p => p.UserId).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(c => c.Contacts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.ParentContact)
                .WithMany(c => c.ChildContacts)
                .HasForeignKey(p => p.ParentContactId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(a => a.UserId).HasName("IX_Contact_UserId");
        }
    }
}