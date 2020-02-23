using Data.Contracts;
using Entities.Post;
using Models.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IPostRepository<TSelect> : IRepository<Post>
    {
        Task<ApiResult<List<TSelect>>> GetAllByCatId(CancellationToken cancellationToken, int id, int to = 0);

        Task<ApiResult<List<TSelect>>> GetSimilar(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<TSelect>>> GetByUserId(CancellationToken cancellationToken, int id);

        Task<ApiResult<List<TSelect>>> GetCustom(CancellationToken cancellationToken, int type, int dateType, int count);

        Task<ApiResult<List<TSelect>>> Search(CancellationToken cancellationToken, string str);
    }
}