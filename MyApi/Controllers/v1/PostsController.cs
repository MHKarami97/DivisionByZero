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
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Like> _repositoryLike;
        private readonly IRepository<PostTag> _repositoryTag;
        private readonly IRepository<Follower> _repositoryFollower;
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<View> _repositoryView;
        private readonly ViewsController _viewsController;

        public PostsController(IRepository<Post> repository, IMapper mapper, UserManager<User> userManager, IRepository<PostTag> repositoryTag, IRepository<Follower> repositoryFollower, IRepository<Like> repositoryLike, IRepository<Comment> repositoryComment, IRepository<View> repositoryView, ViewsController viewsController)
            : base(repository, mapper)
        {
            _userManager = userManager;
            _repositoryTag = repositoryTag;
            _repositoryFollower = repositoryFollower;
            _repositoryLike = repositoryLike;
            _repositoryComment = repositoryComment;
            _repositoryView = repositoryView;
            _viewsController = viewsController;
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
            result.Data.IsLiked = false;
            var isAuthorize = false;

            if (UserIsAutheticated)
            {
                var userId = HttpContext.User.Identity.GetUserId<int>();

                var isFollowed = await _repositoryFollower.TableNoTracking
                    .AnyAsync(a => a.VersionStatus.Equals(2) && a.FollowerId.Equals(result.Data.UserId) && a.UserId.Equals(userId), cancellationToken);

                var isLiked = await _repositoryLike.TableNoTracking
                    .AnyAsync(a => a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id) && a.UserId.Equals(userId), cancellationToken);

                if (isLiked)
                    result.Data.IsLiked = true;

                if (isFollowed)
                    result.Data.IsFollowed = true;

                isAuthorize = true;
            }

            var tags = await _repositoryTag.TableNoTracking
            .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id))
            .Include(a => a.Tag)
            .Select(a => a.Tag)
            .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

            var likes = await _repositoryLike.TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id), cancellationToken);

            var comments = await _repositoryComment.TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id), cancellationToken);

            var views = await _repositoryView.TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(result.Data.Id), cancellationToken);

            result.Data.Tags = tags;
            result.Data.View = views;
            result.Data.Likes = likes;
            result.Data.Comment = comments;

            await _viewsController.IncreaseView(id, isAuthorize, cancellationToken);

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

                if (!item.UserId.Equals(user.Id))
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
            dto.UserId = HttpContext.User.Identity.GetUserId<int>();
            dto.Time = DateTimeOffset.Now;

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

            return list;
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

            return list;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(id))
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return list;
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<PostShortSelectDto>>> GetCustom(CancellationToken cancellationToken, int type = 1, int count = DefaultTake)
        {
            if (count > 30)
                return BadRequest("تعداد درخواست زیاد است");

            switch (type)
            {
                case 1:
                    var list = await Repository.TableNoTracking
                        .Where(a => !a.VersionStatus.Equals(2))
                        .OrderByDescending(a => a.Time)
                        .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                        .Take(count)
                        .ToListAsync(cancellationToken);

                    return list;

                case 2:
                    var result = await _repositoryLike.TableNoTracking
                        .GroupBy(a => a.PostId)
                        .Select(g => new { g.Key, Count = g.Count() })
                        .OrderByDescending(a => a.Count)
                        .Take(count)
                        .ToListAsync(cancellationToken);

                    var ids = result.Select(item => item.Key).ToList();

                    list = await Repository.TableNoTracking
                       .Where(a => !a.VersionStatus.Equals(2))
                       .Where(a => ids.Contains(a.Id))
                       .OrderByDescending(a => a.Time)
                       .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                       .Take(count)
                       .ToListAsync(cancellationToken);

                    return list;

                case 3:
                    result = await _repositoryView.TableNoTracking
                       .GroupBy(a => a.PostId)
                       .Select(g => new { g.Key, Count = g.Count() })
                       .OrderByDescending(a => a.Count)
                       .Take(count)
                       .ToListAsync(cancellationToken);

                    ids = result.Select(item => item.Key).ToList();

                    list = await Repository.TableNoTracking
                        .Where(a => !a.VersionStatus.Equals(2))
                        .Where(a => ids.Contains(a.Id))
                        .OrderByDescending(a => a.Time)
                        .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                        .Take(count)
                        .ToListAsync(cancellationToken);

                    return list;

                case 4:
                    result = await _repositoryComment.TableNoTracking
                        .GroupBy(a => a.PostId)
                        .Select(g => new { g.Key, Count = g.Count() })
                        .OrderByDescending(a => a.Count)
                        .Take(count)
                        .ToListAsync(cancellationToken);

                    ids = result.Select(item => item.Key).ToList();

                    list = await Repository.TableNoTracking
                        .Where(a => !a.VersionStatus.Equals(2))
                        .Where(a => ids.Contains(a.Id))
                        .OrderByDescending(a => a.Time)
                        .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                        .Take(count)
                        .ToListAsync(cancellationToken);

                    return list;

                default:
                    return BadRequest("نوع مطلب درخواستی نامعتبر است");
            }
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

            return list;
        }
    }
}