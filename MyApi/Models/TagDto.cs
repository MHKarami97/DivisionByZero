using Entities.Post;
using WebFramework.Api;

namespace MyApi.Models
{
    public class TagDto : BaseDto<TagDto, Tag>
    {
        public string Name { get; set; }
    }

    public class PostTagDto : BaseDto<PostTagDto, PostTag>
    {
        public int PostId { get; set; }
        public string TagName { get; set; }
    }
}