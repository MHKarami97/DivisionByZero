using Data.Contracts;
using Entities.Employ;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IEmployRepository : IRepository<Employ>
    {
        Task<ApiResult<List<EmploySelectDto>>> GetByUserId(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<EmploySelectDto>>> Get(CancellationToken cancellationToken);
    }
}