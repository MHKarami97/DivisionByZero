using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories;
using Entities.Contact;
using Models.Base;
using Repositories.Contracts;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class ContactRepository<TSelect> : Repository<Contact>, IContactRepository<TSelect>, IScopedDependency
    {
        public ContactRepository(ApplicationDbContext dbContext, IMapper mapper)
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

        public async Task<ApiResult<List<TSelect>>> GetByParentId(CancellationToken cancellationToken, int id, int userId = 0)
        {
            if (userId == 0)
            {
                var list = await TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(id))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
            else
            {
                var list = await TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(id) && a.UserId.Equals(userId))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
        }

        public async Task<ApiResult<List<TSelect>>> Get(CancellationToken cancellationToken, int id = 0)
        {
            if (id == 0)
            {
                var list = await TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(0))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
            else
            {
                var list = await TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(0) && a.UserId.Equals(id))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<TSelect>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }

        }
    }
}