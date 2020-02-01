using Entities.User;
using WebFramework.Api;

namespace MyApi.Models
{
    public class FavoriteDto : BaseDto<FavoriteDto, Favorite>
    {
        public string PostTitle { get; set; }
        public int PostId { get; set; }
    }

    public class FavoriteSelectDto : BaseDto<FavoriteSelectDto, Favorite>
    {
        public int PostId { get; set; }
    }
}