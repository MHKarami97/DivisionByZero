﻿using Entities.Post;
using WebFramework.Api;

namespace MyApi.Models
{
    public class CategoryDto : BaseDto<CategoryDto, Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; }
    }

    public class CategoryCreateDto : BaseDto<CategoryCreateDto, Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
