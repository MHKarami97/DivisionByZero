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
using System.Data;
using System.Linq;

namespace Repositories.Repositories
{
    public class LikeRepository : Repository<Like>, ILikeRepository, IScopedDependency, IBaseRepository
    {
        public LikeRepository(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<bool> LikePost(int userId, int id, float rate, CancellationToken cancellationToken)
        {
            if (rate < 0 || rate > 5)
                throw new DataException("مقدار امتیاز نامعتبر است");

            var isLike = await TableNoTracking
                .AnyAsync(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId) && a.PostId.Equals(id), cancellationToken);

            if (isLike)
                throw new DataException("این مطلب قبلا امتیاز دهی شده است");

            await AddAsync(new Like
            {
                PostId = id,
                Rate = rate,
                UserId = userId,
                Time = DateTimeOffset.Now
            }, cancellationToken);

            return true;
        }

        public async Task<bool> DisLikePost(int userId, int id, CancellationToken cancellationToken)
        {
            var result = await TableNoTracking
                .SingleAsync(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId) && a.PostId.Equals(id), cancellationToken);

            await DeleteAsync(result, cancellationToken);

            return true;
        }

        public async Task<ApiResult<List<LikeDto>>> Get(int userId, CancellationToken cancellationToken)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId))
                .ProjectTo<LikeDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}