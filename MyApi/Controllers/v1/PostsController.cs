using MyApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities.Post;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PostsController : CrudController<PostDto, PostSelectDto, Post>
    {
        private const int DefaultTake = 7;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<PostTag> _repositoryTag;
        private readonly IRepository<Follower> _repositoryFollower;

        public PostsController(IRepository<Post> repository, IMapper mapper, UserManager<User> userManager, IRepository<PostTag> repositoryTag, IRepository<Follower> repositoryFollower)
            : base(repository, mapper)
        {
            _userManager = userManager;
            _repositoryTag = repositoryTag;
            _repositoryFollower = repositoryFollower;
        }

        [NonAction]
        public override Task<ApiResult<List<PostSelectDto>>> Get(CancellationToken cancellationToken)
        {
            return base.Get(cancellationToken);
        }

        [AllowAnonymous]
        public override async Task<ApiResult<PostSelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            var result = await base.Get(id, cancellationToken);

            result.Data.IsFollowed = false;

            if (UserIsAutheticated)
            {
                var userId = HttpContext.User.Identity.GetUserId<int>();

                var isFollowed = await _repositoryFollower.TableNoTracking
                    .AnyAsync(a => a.FollowerId.Equals(result.Data.AuthorId) && a.UserId.Equals(userId), cancellationToken: cancellationToken);

                if (isFollowed)
                    result.Data.IsFollowed = true;
            }

            var tags = await _repositoryTag.TableNoTracking
            .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id))
            .Include(a => a.Tag)
            .Select(a => a.Tag)
            .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

            result.Data.Tags = tags;

            var dto = await Repository.Table
                .Where(a => !a.VersionStatus.Equals(2) && a.Id.Equals(id))
                .OrderByDescending(a => a.Version)
                .FirstOrDefaultAsync(cancellationToken);

            dto.View += 1;

            await Repository.UpdateAsync(dto, cancellationToken);

            return result;
        }

        public override async Task<ApiResult<PostSelectDto>> Update(int id, PostDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return await base.Update(id, dto, cancellationToken);
            }

            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                var item = await Repository.TableNoTracking.SingleAsync(a => a.Id.Equals(id), cancellationToken);

                if (!item.AuthorId.Equals(user.Id))
                    return BadRequest();

                return await base.Update(id, dto, cancellationToken);
            }

            return BadRequest();
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ApiResult<PostSelectDto>> Create(PostDto dto, CancellationToken cancellationToken)
        {
            dto.AuthorId = HttpContext.User.Identity.GetUserId<int>();
            dto.Time = DateTime.Now;

            return base.Create(dto, cancellationToken);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetAllByCatId(CancellationToken cancellationToken, int id, int to = 0)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.CategoryId.Equals(id))
                .OrderByDescending(a => a.Time)
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake + to)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetSimilar(int id, CancellationToken cancellationToken)
        {
            var post = await Repository.TableNoTracking
                .Where(a => a.Id.Equals(id))
                .SingleAsync(cancellationToken);

            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.CategoryId.Equals(post.CategoryId))
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.AuthorId.Equals(id))
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetCustom(CancellationToken cancellationToken, int type = 1, int count = DefaultTake)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .OrderByDescending(a => a.Time)
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(count)
                .ToListAsync(cancellationToken);

            switch (type)
            {
                case 1:
                    break;
                case 2:
                    list = list.OrderByDescending(a => a.Rank).ToList();
                    break;
                case 3:
                    list = list.OrderByDescending(a => a.View).ToList();
                    break;
            }

            return Ok(list);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> Search(string str, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(str))
                return BadRequest("کلمه مورد جستجو خالی است");

            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.Title.Contains(str))
                .OrderByDescending(a => a.Time)
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }
    }
}