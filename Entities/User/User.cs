﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.Common;
using Entities.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class User : IdentityUser<int>, IEntity
    {
        public User()
        {
            IsActive = true;
        }

        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public GenderType Gender { get; set; }
        public bool IsActive { get; set; }
        public int? VerifyCode { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

        public ICollection<Post.Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Employ.Employ> Employs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Follower> Followers { get; set; }
        public ICollection<Contact.Contact> Contacts { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<View> Views { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
        }
    }

    public enum GenderType
    {
        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2
    }
}
