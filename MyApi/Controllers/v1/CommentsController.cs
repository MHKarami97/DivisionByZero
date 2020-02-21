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
using Services.Security;
using System;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class CommentsController : CrudController<CommentDto, CommentSelectDto, Comment>
    {
        private readonly IMapper _mapper;
        private readonly ISecurity _security;

        public CommentsController(IRepository<Comment> repository, IMapper mapper, ISecurity security)
            : base(repository, mapper)
        {
            _mapper = mapper;
            _security = security;
        }

        [AllowAnonymous]
        public override Task<ApiResult<CommentSelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ApiResult<List<CommentSelectDto>>> GetPostComments(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .ProjectTo<CommentSelectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<CommentSelectDto>>> GetLastComments(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .ProjectTo<CommentSelectDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(a=>a.Time)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return list;
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

        public override async Task<ApiResult<CommentSelectDto>> Create(CommentDto dto, CancellationToken cancellationToken)
        {
            dto.UserId = HttpContext.User.Identity.GetUserId<int>();
            dto.Time = DateTimeOffset.Now;

            var lastComment = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(dto.PostId) && a.UserId.Equals(dto.UserId))
                .OrderByDescending(a => a.Time)
                .Select(a => a.Time)
                .FirstAsync(cancellationToken);

            if (!_security.TimeCheck(lastComment))
                return BadRequest("لطفا کمی صبر کنید و بعد نظر بدهید");

            return await base.Create(dto, cancellationToken);
        }
    }
}