using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PostsController : CrudController<PostDto, PostSelectDto, Post>
    {
        public PostsController(IRepository<Post> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
