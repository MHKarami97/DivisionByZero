using Entities.More;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class HelpDto : BaseDto<HelpDto, Help>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
    }

    public class HelpSelectDto : BaseDto<HelpSelectDto, Help>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
