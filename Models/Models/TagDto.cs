using Entities.Post;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models.Base;

namespace Models.Models
{
    public class TagDto : BaseDto<TagDto, Tag>
    {
        public string Name { get; set; }
    }

    public class PostTagDto : BaseDto<PostTagDto, PostTag>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public List<string> TagName { get; set; }
    }
}