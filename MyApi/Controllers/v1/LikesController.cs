using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities.Post;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class LikesController : BaseController
    {
        private readonly IRepository<Like> _repositoryLike;
        private readonly IRepository<Post> _repositoryPost;
        private readonly IMapper _mapper;

        public LikesController(IRepository<Like> repositoryLike, IRepository<Post> repositoryPost, IMapper mapper)
        {
            _repositoryLike = repositoryLike;
            _repositoryPost = repositoryPost;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> LikePost(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var isPostExist = await _repositoryPost.TableNoTracking
                .AnyAsync(a => !a.VersionStatus.Equals(2) && a.Id.Equals(id), cancellationToken);

            if (!isPostExist)
                return BadRequest("مطلب موجود نمی باشد");

            var isLike = await _repositoryLike.TableNoTracking
                .AnyAsync(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId) && a.PostId.Equals(id), cancellationToken);

            if (isLike)
                return BadRequest("این مطلب قبلا لایک شده است");

            await _repositoryLike.AddAsync(new Like
            {
                PostId = id,
                UserId = userId,
                Time = DateTimeOffset.Now
            }, cancellationToken);

            return Ok("با موفقیت انجام شد");
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> DisLikePost(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var result = await _repositoryLike.TableNoTracking
                .SingleAsync(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId) && a.PostId.Equals(id), cancellationToken);

            await _repositoryLike.DeleteAsync(result, cancellationToken);

            return Ok("با موفقیت انجام شد");
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<LikeDto>>> Get(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var list = await _repositoryLike.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId))
                .ProjectTo<ViewDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }
    }
}