using AutoMapper;
using Entities.Contact;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class ContactDto : BaseDto<ContactDto, Contact>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int ParentContactId { get; set; }

        [JsonIgnore]
        public DateTimeOffset Time { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public bool ByServer { get; set; }
    }

    public class ContactSelectDto : BaseDto<ContactSelectDto, Contact>
    {
        public string Text { get; set; }
        public string Time { get; set; }
        public string UserFullName { get; set; }
        public bool ByServer { get; set; }
        public int ParentContactId { get; set; }

        public override void CustomMappings(IMappingExpression<Contact, ContactSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.Time,
                config => config.MapFrom(src => src.Time.ToString("g")));
        }
    }
}
