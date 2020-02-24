using Data.Contracts;
using Entities.User;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IViewRepository : IRepository<View>
    {
        Task<bool> IncreaseView(int? userId, string ip, int id, CancellationToken cancellationToken);

        Task<ApiResult<List<ViewDto>>> Get(int userId, CancellationToken cancellationToken);
    }
}