using AutoMapper;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
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
        private readonly ITagRepository _tagRepository;
        private readonly IRepository<PostTag> _repositoryPostTag;

        public TagsController(IRepository<Tag> repository, IMapper mapper, IRepository<PostTag> repositoryPostTag, ITagRepository tagRepository)
            : base(repository, mapper)
        {
            _repositoryPostTag = repositoryPostTag;
            _tagRepository = tagRepository;
        }

        [AllowAnonymous]
        public override async Task<ApiResult<List<TagDto>>> Get(CancellationToken cancellationToken)
        {
            return await _tagRepository.Get(cancellationToken);
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
            if (await _tagRepository.AddTag(dto, cancellationToken))
                return Ok("با موفقیت ایجاد شد");

            return BadRequest("خطا در ثبت تگ");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<TagDto>>> GetPostTag(int id, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetPostTag(id, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<List<PostDto>>> GetPostsInTag(string tag, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetPostsInTag(tag, cancellationToken);
        }

        [NonAction]
        public override Task<ApiResult<TagDto>> Create(TagDto dto, CancellationToken cancellationToken)
        {
            return base.Create(dto, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var postTags = await _repositoryPostTag.Table.Where(a => a.TagId.Equals(id)).ToListAsync(cancellationToken);

            await _repositoryPostTag.DeleteRangeAsync(postTags, cancellationToken);

            return await base.Delete(id, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<TagDto>> Update(int id, TagDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }
    }
}