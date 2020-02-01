using Entities.User;
using WebFramework.Api;

namespace MyApi.Models
{
    public class FollowerDto : BaseDto<FollowerDto, Follower>
    {
        public string UserFullName { get; set; }
        public int UserId { get; set; }
    }

    public class FollowerSelectDto : BaseDto<FollowerSelectDto, Follower>
    {
        public int UserId { get; set; }
    }
}