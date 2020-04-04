using AutoMapper;
using Entities.Post;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class PostDto : BaseDto<PostDto, Post>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Html)]
        public string Text { get; set; }

        [Required]
        [StringLength(500)]
        [DataType(DataType.Text)]
        public string ShortDescription { get; set; }

        [Required]
        public int TimeToRead { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public DateTimeOffset Time { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class PostShortSelectDto : BaseDto<PostShortSelectDto, Post>
    {
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string UserFullName { get; set; }
        public int UserId { get; set; }
        public string Time { get; set; }
        public string ShortDescription { get; set; }
        public int TimeToRead { get; set; }
        public string Image { get; set; }

        public override void CustomMappings(IMappingExpression<Post, PostShortSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));
        }
    }

    public class PostSelectDto : BaseDto<PostSelectDto, Post>
    {
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string UserFullName { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public string ShortDescription { get; set; }
        public int TimeToRead { get; set; }
        public string Image { get; set; }

        [IgnoreMap]
        public int View { get; set; }

        [IgnoreMap]
        public float Likes { get; set; }

        [IgnoreMap]
        public int Comment { get; set; }

        [IgnoreMap]
        public bool IsFollowed { get; set; }

        [IgnoreMap]
        public bool IsLiked { get; set; }

        [IgnoreMap]
        public List<TagDto> Tags { get; set; }

        public override void CustomMappings(IMappingExpression<Post, PostSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));

            mappingExpression.ForMember(
                dest => dest.IsFollowed,
                config => config.MapFrom(src => false));
        }
    }
}