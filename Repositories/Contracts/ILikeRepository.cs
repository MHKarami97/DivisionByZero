using Data.Contracts;
using Entities.User;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ILikeRepository : IRepository<Like>
    {
        Task<bool> LikePost(int userId, int id, float rate, CancellationToken cancellationToken);

        Task<bool> DisLikePost(int userId, int id, CancellationToken cancellationToken);

        Task<ApiResult<List<LikeDto>>> Get(int userId, CancellationToken cancellationToken);
    }
}