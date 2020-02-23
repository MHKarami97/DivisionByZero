using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories;
using Entities.Employ;
using Models.Base;
using Repositories.Contracts;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class EmployRepository<TSelect> : Repository<Employ>, IEmployRepository<TSelect>, IScopedDependency
    {
        public EmployRepository(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<ApiResult<List<TSelect>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(id))
                .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<TSelect>>> Get(CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .OrderByDescending(a => a.Time)
                .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}