using Entities.User;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class FollowerDto : BaseDto<FollowerDto, Follower>
    {
        public string FollowersFullName { get; set; }
        public int FollowerId { get; set; }
    }

    public class FollowerSelectDto : BaseDto<FollowerSelectDto, Follower>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        public int FollowerId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}