using Entities.Post;
using Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.Models
{
    public class CategoryDto : BaseDto<CategoryDto, Category>
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public int? ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; }
    }

    public class ShortCategoryDto : BaseDto<ShortCategoryDto, Category>
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class CategoryCreateDto : BaseDto<CategoryCreateDto, Category>
    {
        [JsonIgnore]
        public override int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public string Image { get; set; }

        public int? ParentCategoryId { get; set; }
    }

    public class CategoryWithSubCatDto : BaseDto<CategoryWithSubCatDto, Category>
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ShortCategoryDto> ChildCategories { get; set; }
    }
}
