using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories;
using Entities.Post;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository, IScopedDependency, IBaseRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<ApiResult<List<CategoryDto>>> GetAllMainCat(CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(0) || a.ParentCategoryId == null)
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<CategoryWithSubCatDto>>> GetCategoryWithSub(CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(0) || a.ParentCategoryId == null)
                .Include(a => a.ChildCategories)
                .ProjectTo<CategoryWithSubCatDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<CategoryDto>>> GetAllByCatId(int id, CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(id))
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}