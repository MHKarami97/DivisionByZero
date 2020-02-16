using AutoMapper;
using Entities.User;
using System;
using WebFramework.Api;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class LikeDto : BaseDto<LikeDto, Like>
    {
        public string PostTitle { get; set; }

        public string Time { get; set; }

        public override void CustomMappings(IMappingExpression<Like, LikeDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));
        }
    }

    public class LikeSelectDto : BaseDto<LikeSelectDto, Like>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public DateTimeOffset Time { get; set; }
    }
}