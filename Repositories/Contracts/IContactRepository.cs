using Data.Contracts;
using Entities.Contact;
using Models.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IContactRepository<TSelect> : IRepository<Contact>
    {
        Task<ApiResult<List<TSelect>>> GetByUserId(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<TSelect>>> GetByParentId(CancellationToken cancellationToken, int id, int userId = 0);

        Task<ApiResult<List<TSelect>>> Get(CancellationToken cancellationToken, int id = 0);
    }
}