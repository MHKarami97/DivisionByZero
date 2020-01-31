using System;
using AutoMapper;
using Entities.Post;
using WebFramework.Api;

namespace MyApi.Models
{
    public class CommentDto : BaseDto<CommentDto, Comment>
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }

        public override void CustomMappings(IMappingExpression<Comment, CommentDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => DateTime.Now));
        }
    }

    public class CommentSelectDto : BaseDto<CommentSelectDto, Comment>
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public int UserFullName { get; set; }
        public DateTime Time { get; set; }
    }
}