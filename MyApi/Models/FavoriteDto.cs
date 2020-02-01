using Entities.Post;
using Entities.User;
using WebFramework.Api;

namespace MyApi.Models
{
    public class FavoriteDto : BaseDto<FavoriteDto, Favorite>
    {
        public string PostTitle { get; set; }
        public int PostId { get; set; }
    }

    public class FavoriteDtoSelectDto : BaseDto<FavoriteDtoSelectDto, Comment>
    {
        public int PostId { get; set; }
    }
}