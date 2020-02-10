using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using MyApi.Models;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        [AllowAnonymous]
        public override Task<ApiResult<CommentSelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<CommentSelectDto>>> GetPostComments(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .ProjectTo<CommentSelectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<List<CommentSelectDto>>> Get(CancellationToken cancellationToken)
        {
            return base.Get(cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<CommentSelectDto>> Update(int id, CommentDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        public override Task<ApiResult<CommentSelectDto>> Create(CommentDto dto, CancellationToken cancellationToken)
        {
            dto.UserId = HttpContext.User.Identity.GetUserId<int>();
            dto.Time = DateTime.Now;

            return base.Create(dto, cancellationToken);
        }
    }
}