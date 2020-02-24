using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories;
using Entities.Post;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository, IScopedDependency, IBaseRepository
    {
        private readonly IRepository<PostTag> _repositoryPostTag;
        private readonly IRepository<Post> _repositoryPost;

        public TagRepository(ApplicationDbContext dbContext, IMapper mapper, IRepository<PostTag> repositoryPostTag, IRepository<Post> repositoryPost)
            : base(dbContext, mapper)
        {
            _repositoryPostTag = repositoryPostTag;
            _repositoryPost = repositoryPost;
        }

        public async Task<bool> AddTag(PostTagDto dto, CancellationToken cancellationToken)
        {
            Assert.NotEmpty(dto.TagName, "list", "لیست تگ ها خالی است");

            Assert.NotNull(dto.PostId, "آی دی پست نامعتبر است");

            foreach (var tagDto in dto.TagName)
            {
                var isTagExist = await TableNoTracking.SingleOrDefaultAsync(a => a.Name.Equals(tagDto), cancellationToken);

                if (isTagExist == null)
                {
                    var newTag = new Tag
                    {
                        Name = tagDto,
                        VersionStatus = 0,
                        Version = 1
                    };

                    await AddAsync(newTag, cancellationToken);

                    var postTag = new PostTag
                    {
                        PostId = dto.PostId,
                        TagId = newTag.Id,
                        VersionStatus = 0,
                        Version = 1
                    };

                    await _repositoryPostTag.AddAsync(postTag, cancellationToken, false);
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

                    await _repositoryPostTag.AddAsync(postTag, cancellationToken, false);
                }
            }

            var save = await DbContext.SaveChangesAsync(cancellationToken);

            return Convert.ToBoolean(save);
        }

        public async Task<ApiResult<List<TagDto>>> GetPostTag(int id, CancellationToken cancellationToken)
        {
            var list = await _repositoryPostTag.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .Select(a => a.TagId)
                .ToListAsync(cancellationToken);

            var result = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && list.Contains(a.Id))
                .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<ApiResult<List<PostDto>>> GetPostsInTag(string tag, CancellationToken cancellationToken)
        {
            Assert.NotNullArgument(tag, "آی دی پست اشتباه است");

            var getTag = await TableNoTracking
                .SingleAsync(a => a.Name.Equals(tag), cancellationToken);

            var postTags = await _repositoryPostTag.TableNoTracking
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

        public async Task<ApiResult<List<TagDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await _repositoryPostTag.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .GroupBy(a => a.TagId)
                .Select(g => new { g.Key, Count = g.Count() })
                .OrderByDescending(a => a.Count)
                .Take(20)
                .ToListAsync(cancellationToken);

            var result = new List<TagDto>();

            foreach (var value in list)
            {
                var res = await TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.Id.Equals(value.Key))
                    .ProjectTo<TagDto>(Mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(cancellationToken);

                if (res == null)
                    continue;

                result.Add(res);
            }

            return result;
        }
    }
}