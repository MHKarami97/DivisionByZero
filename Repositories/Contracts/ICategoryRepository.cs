using Data.Contracts;
using Entities.Post;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<ApiResult<List<CategoryDto>>> GetAllMainCat(CancellationToken cancellationToken);

        Task<ApiResult<List<CategoryWithSubCatDto>>> GetCategoryWithSub(CancellationToken cancellationToken);

        Task<ApiResult<List<CategoryDto>>> GetAllByCatId(int id, CancellationToken cancellationToken);
    }
}