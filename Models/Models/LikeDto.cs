using AutoMapper;
using Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class LikeDto : BaseDto<LikeDto, Like>
    {
        public string PostTitle { get; set; }

        public string Time { get; set; }

        public float Rate { get; set; }

        public override void CustomMappings(IMappingExpression<Like, LikeDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));
        }
    }

    public class LikeShortDto : BaseDto<LikeShortDto, Like>
    {
        public string UserFullName { get; set; }

        public string Time { get; set; }

        public float Rate { get; set; }

        public override void CustomMappings(IMappingExpression<Like, LikeShortDto> mappingExpression)
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

        [Required]
        public float Rate { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public DateTimeOffset Time { get; set; }
    }
}