using AutoMapper;
using Entities.User;
using System;
using WebFramework.Api;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class ViewDto : BaseDto<ViewDto, View>
    {
        public string PostTitle { get; set; }

        public string Time { get; set; }

        public override void CustomMappings(IMappingExpression<View, ViewDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("d")));
        }
    }

    public class ViewSelectDto : BaseDto<ViewSelectDto, View>
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