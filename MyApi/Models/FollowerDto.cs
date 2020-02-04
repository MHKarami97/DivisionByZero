using Entities.User;
using WebFramework.Api;

namespace MyApi.Models
{
    public class FollowerDto : BaseDto<FollowerDto, Follower>
    {
        public string FollowerFullName { get; set; }
        public int FollowerId { get; set; }
    }

    public class FollowerSelectDto : BaseDto<FollowerSelectDto, Follower>
    {
        public int FollowerId { get; set; }
        public int UserId { get; set; }
    }
}