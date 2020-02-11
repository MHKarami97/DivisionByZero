using Entities.More;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebFramework.Api;

namespace MyApi.Models
{
    public class BannerDto : BaseDto<BannerDto, Banner>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Image { get; set; }

        public string Address { get; set; }
    }

    public class BannerSelectDto : BaseDto<BannerSelectDto, Banner>
    {
        public string Image { get; set; }
        public string Address { get; set; }
    }
}
