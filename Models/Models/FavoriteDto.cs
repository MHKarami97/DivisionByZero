using Entities.User;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class FavoriteDto : BaseDto<FavoriteDto, Favorite>
    {
        public string PostTitle { get; set; }
        public int PostView { get; set; }
        public int PostRank { get; set; }
        public string PostShortDescription { get; set; }
        public string Image { get; set; }
        public int PostId { get; set; }
    }

    public class FavoriteSelectDto : BaseDto<FavoriteSelectDto, Favorite>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}