using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MyApi.Models;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class CommentsController : CrudController<CommentDto, CommentSelectDto, Comment>
    {
        private readonly IMapper _mapper;

        public CommentsController(IRepository<Comment> repository, IMapper mapper)
            : base(repository, mapper)
        {
            _mapper = mapper;
        }
    }
}