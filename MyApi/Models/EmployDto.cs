using System;
using AutoMapper;
using Entities.Employ;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebFramework.Api;

namespace MyApi.Models
{
    public class EmployDto : BaseDto<EmployDto, Employ>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(200)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Html)]
        public string Text { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        public override void CustomMappings(IMappingExpression<Employ, EmployDto> mappingExpression)
        {
            // mappingExpression.ForMember(
            //     dest => dest.Time,
            //     config => config.MapFrom(src => DateTime.Now));
        }
    }

    public class EmploySelectDto : BaseDto<EmploySelectDto, Employ>
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public string UserFullName { get; set; }
    }
}
