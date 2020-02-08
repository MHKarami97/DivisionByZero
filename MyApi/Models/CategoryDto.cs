using Entities.Post;
using System.Collections.Generic;
using WebFramework.Api;

namespace MyApi.Models
{
    public class CategoryDto : BaseDto<CategoryDto, Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; }
    }

    public class ShortCategoryDto : BaseDto<ShortCategoryDto, Category>
    {
        public string Name { get; set; }
    }

    public class CategoryCreateDto : BaseDto<CategoryCreateDto, Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }

    public class CategoryWithSubCatDto : BaseDto<CategoryWithSubCatDto, Category>
    {
        public string Name { get; set; }
        public List<ShortCategoryDto> Sub { get; set; }
    }
}
