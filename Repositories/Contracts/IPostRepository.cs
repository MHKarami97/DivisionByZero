using Data.Contracts;
using Entities.Post;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<ApiResult<List<PostShortSelectDto>>> GetAllByCatId(CancellationToken cancellationToken, int id, int to = 0);

        Task<ApiResult<List<PostShortSelectDto>>> GetSimilar(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<PostShortSelectDto>>> GetByUserId(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<ViewShortDto>>> GetView(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<LikeShortDto>>> GetLike(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<PostShortSelectDto>>> GetCustom(CancellationToken cancellationToken, int type, int dateType, int count);

        Task<ApiResult<List<PostShortSelectDto>>> Search(CancellationToken cancellationToken, string str);
    }
}