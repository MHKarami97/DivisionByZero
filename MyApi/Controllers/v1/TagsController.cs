using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class TagsController : CrudController<TagDto, Tag>
    {
        private readonly IRepository<PostTag> _repositoryTag;
        private readonly IRepository<Post> _repositoryPost;
        protected readonly ApplicationDbContext DbContext;

        public TagsController(IRepository<Tag> repository, IMapper mapper, IRepository<PostTag> repositoryTag, ApplicationDbContext dbContext, IRepository<Post> repositoryPost)
            : base(repository, mapper)
        {
            _repositoryTag = repositoryTag;
            DbContext = dbContext;
            _repositoryPost = repositoryPost;
        }

        [AllowAnonymous]
        public override async Task<ApiResult<List<TagDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await _repositoryTag.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .GroupBy(a => a.TagId)
                .Select(g => new { g.Key, Count = g.Count() })
                .OrderByDescending(a => a.Count)
                .Take(20)
                .ToListAsync(cancellationToken);

            var result = new List<TagDto>();

            foreach (var value in list)
            {
                var res = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.Id.Equals(value.Key))
                    .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(cancellationToken);

                if (res == null)
                    continue;

                result.Add(res);
            }

            return result;
        }

        [NonAction]
        public override Task<ApiResult<TagDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddTag(PostTagDto dto, CancellationToken cancellationToken)
        {
            if (!dto.TagName.Any())
                return BadRequest("لیست تگ ها خالی است");

            if (dto.PostId == 0)
                return BadRequest("آی دی پست نامعتبر است");

            foreach (var tagDto in dto.TagName)
            {
                var isTagExist = await Repository.TableNoTracking.SingleOrDefaultAsync(a => a.Name.Equals(tagDto), cancellationToken);

                if (isTagExist == null)
                {
                    var newTag = new Tag
                    {
                        Name = tagDto,
                        VersionStatus = 0,
                        Version = 1
                    };

                    await Repository.AddAsync(newTag, cancellationToken);

                    var postTag = new PostTag
                    {
                        PostId = dto.PostId,
                        TagId = newTag.Id,
                        VersionStatus = 0,
                        Version = 1
                    };

                    await _repositoryTag.AddAsync(postTag, cancellationToken, false);
                }
                else
                {
                    var postTag = new PostTag
                    {
                        PostId = dto.PostId,
                        TagId = isTagExist.Id,
                        VersionStatus = 0,
                        Version = 1
                    };

                    await _repositoryTag.AddAsync(postTag, cancellationToken, false);
                }
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            return Ok("با موفقیت ایجاد شد");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<TagDto>>> GetPostTag(int id, CancellationToken cancellationToken)
        {
            if (id.Equals(0))
                return BadRequest("آی دی پست اشتباه است");

            var list = await _repositoryTag.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<PostDto>>> GetPostsInTag(string tag, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(tag))
                return BadRequest("آی دی پست اشتباه است");

            var getTag = await Repository.TableNoTracking
                .SingleAsync(a => a.Name.Equals(tag), cancellationToken);

            var postTags = await _repositoryTag.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.TagId.Equals(getTag.Id))
                .Select(a => a.PostId)
                .ToListAsync(cancellationToken);

            var list = await _repositoryPost.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .Where(a => postTags.Contains(a.Id))
                .ProjectTo<PostDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [NonAction]
        public override Task<ApiResult<TagDto>> Create(TagDto dto, CancellationToken cancellationToken)
        {
            return base.Create(dto, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var postTags = await _repositoryTag.Table.Where(a => a.TagId.Equals(id)).ToListAsync(cancellationToken);

            await _repositoryTag.DeleteRangeAsync(postTags, cancellationToken);

            return await base.Delete(id, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<TagDto>> Update(int id, TagDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }
    }
}