using Data.Contracts;
using Entities.Contact;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<ApiResult<List<ContactSelectDto>>> GetByUserId(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<ContactSelectDto>>> GetByParentId(CancellationToken cancellationToken, int id, int userId = 0);

        Task<ApiResult<List<ContactSelectDto>>> Get(CancellationToken cancellationToken, int id = 0);
    }
}