using Data.Contracts;
using Entities.Post;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<bool> AddTag(PostTagDto dto, CancellationToken cancellationToken);

        Task<ApiResult<List<TagDto>>> GetPostTag(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<PostDto>>> GetPostsInTag(string tag, CancellationToken cancellationToken);

        Task<ApiResult<List<TagDto>>> Get(CancellationToken cancellationToken);
    }
}