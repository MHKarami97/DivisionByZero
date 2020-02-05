using System;
using AutoMapper;
using Entities.Employ;
using WebFramework.Api;

namespace MyApi.Models
{
    public class EmployDto : BaseDto<EmployDto, Employ>
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }

        public override void CustomMappings(IMappingExpression<Employ, EmployDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => DateTime.Now));
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
