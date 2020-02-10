using System;
using AutoMapper;
using Entities.Post;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebFramework.Api;

namespace MyApi.Models
{
    public class CommentDto : BaseDto<CommentDto, Comment>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        public int PostId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }

        public override void CustomMappings(IMappingExpression<Comment, CommentDto> mappingExpression)
        {
            // mappingExpression.ForMember(
            //     dest => dest.Time,
            //     config => config.MapFrom(src => DateTime.Now));
        }
    }

    public class CommentSelectDto : BaseDto<CommentSelectDto, Comment>
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public DateTime Time { get; set; }
    }
}