using AutoMapper;
using Entities.Post;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebFramework.Api;

namespace MyApi.Models
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
        public DateTime TimeToRead { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }

        [JsonIgnore]
        public int AuthorId { get; set; }

        public override void CustomMappings(IMappingExpression<Post, PostDto> mappingExpression)
        {
            // mappingExpression.ForMember(
            //     dest => dest.Time,
            //     config => config.MapFrom(src => DateTime.Now));
        }
    }

    public class PostShortSelectDto : BaseDto<PostShortSelectDto, Post>
    {
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string AuthorFullName { get; set; }
        public DateTime Time { get; set; }
        public string ShortDescription { get; set; }
        public string TimeToRead { get; set; }
        public string Image { get; set; }
        public int View { get; set; }
        public int Rank { get; set; }

        public override void CustomMappings(IMappingExpression<Post, PostShortSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.TimeToRead,
                config => config.MapFrom(src => src.TimeToRead.ToString("t")));
        }
    }

    public class PostSelectDto : BaseDto<PostSelectDto, Post>
    {
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string AuthorFullName { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public string ShortDescription { get; set; }
        public string TimeToRead { get; set; }
        public string Image { get; set; }
        public int View { get; set; }
        public int Rank { get; set; }

        public override void CustomMappings(IMappingExpression<Post, PostSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.TimeToRead,
                    config => config.MapFrom(src => src.TimeToRead.ToString("t")));
        }
    }
}
