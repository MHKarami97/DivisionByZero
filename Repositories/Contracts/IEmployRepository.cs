using Data.Contracts;
using Entities.Employ;
using Models.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IEmployRepository<TSelect> : IRepository<Employ>
    {
        Task<ApiResult<List<TSelect>>> GetByUserId(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<TSelect>>> Get(CancellationToken cancellationToken);
    }
}