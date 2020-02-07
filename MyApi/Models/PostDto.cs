using AutoMapper;
using Entities.Post;
using System;
using WebFramework.Api;

namespace MyApi.Models
{
    public class PostDto : BaseDto<PostDto, Post>
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string ShortDescription { get; set; }
        public DateTime TimeToRead { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
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
