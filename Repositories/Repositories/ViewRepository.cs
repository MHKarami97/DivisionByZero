using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories;
using Entities.User;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Repositories
{
    public class ViewRepository : Repository<View>, IViewRepository, IScopedDependency, IBaseRepository
    {
        public ViewRepository(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<bool> IncreaseView(int? userId, string ip, int id, CancellationToken cancellationToken)
        {
            await AddAsync(new View
            {
                PostId = id,
                UserId = userId,
                Ip = ip,
                Time = DateTimeOffset.Now
            }, cancellationToken);

            return true;
        }

        public async Task<ApiResult<List<ViewDto>>> Get(int userId, CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId))
                .ProjectTo<ViewDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}