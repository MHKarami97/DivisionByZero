using System;
using AutoMapper;
using Models.Base;
using Entities.Post;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Models.Models
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
        public DateTimeOffset Time { get; set; }
    }

    public class CommentSelectDto : BaseDto<CommentSelectDto, Comment>
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string Time { get; set; }

        public override void CustomMappings(IMappingExpression<Comment, CommentSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));
        }
    }
}