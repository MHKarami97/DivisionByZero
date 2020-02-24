using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Data.Repositories;
using Entities.Post;
using Entities.User;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Data;

namespace Repositories.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository, IScopedDependency, IBaseRepository
    {
        private readonly IRepository<Like> _repositoryLike;
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<View> _repositoryView;

        public PostRepository(ApplicationDbContext dbContext, IMapper mapper, IRepository<Like> repositoryLike, IRepository<Comment> repositoryComment, IRepository<View> repositoryView)
            : base(dbContext, mapper)
        {
            _repositoryLike = repositoryLike;
            _repositoryComment = repositoryComment;
            _repositoryView = repositoryView;
        }

        public async Task<ApiResult<List<PostShortSelectDto>>> GetAllByCatId(CancellationToken cancellationToken, int id, int to = 0)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.CategoryId.Equals(id))
                .OrderByDescending(a => a.Time)
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake + to)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<PostShortSelectDto>>> GetSimilar(CancellationToken cancellationToken, int id)
        {
            var post = await TableNoTracking
                .Where(a => a.Id.Equals(id))
                .SingleAsync(cancellationToken);

            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.CategoryId.Equals(post.CategoryId))
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<PostShortSelectDto>>> GetByUserId(CancellationToken cancellationToken, int id)
        {
            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(id))
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<ApiResult<List<ViewShortDto>>> GetView(CancellationToken cancellationToken, int id)
        {
            var result = await _repositoryView.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .OrderByDescending(a => a.Time)
                .ProjectTo<ViewShortDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<ApiResult<List<LikeShortDto>>> GetLike(CancellationToken cancellationToken, int id)
        {
            var result = await _repositoryLike.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id))
                .OrderByDescending(a => a.Time)
                .ProjectTo<LikeShortDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<ApiResult<List<PostShortSelectDto>>> GetCustom(CancellationToken cancellationToken, int type, int dateType, int count)
        {
            if (count > 30)
                throw new DataException("تعداد درخواست زیاد است");

            var today = DateTimeOffset.Now;
            var week = today.AddDays(-7);

            switch (dateType)
            {
                case 1:
                    switch (type)
                    {
                        case 1:
                            var list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 2:
                            var result = await _repositoryLike.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            var ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                               .Where(a => !a.VersionStatus.Equals(2))
                               .Where(a => ids.Contains(a.Id))
                               .OrderByDescending(a => a.Time)
                               .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            return list;

                        case 3:
                            result = await _repositoryView.TableNoTracking
                               .GroupBy(a => a.PostId)
                               .Select(g => new { g.Key, Count = g.Count() })
                               .OrderByDescending(a => a.Count)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 4:
                            result = await _repositoryComment.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        default:
                            throw new DataException("نوع مطلب درخواستی نامعتبر است");
                    }

                case 2:
                    switch (type)
                    {
                        case 1:
                            var list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => a.Time.Year == today.Year && a.Time.Month == today.Month)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 2:
                            var result = await _repositoryLike.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            var ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                               .Where(a => !a.VersionStatus.Equals(2))
                               .Where(a => ids.Contains(a.Id))
                               .Where(a => a.Time.Year == today.Year && a.Time.Month == today.Month)
                               .OrderByDescending(a => a.Time)
                               .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            return list;

                        case 3:
                            result = await _repositoryView.TableNoTracking
                               .GroupBy(a => a.PostId)
                               .Select(g => new { g.Key, Count = g.Count() })
                               .OrderByDescending(a => a.Count)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .Where(a => a.Time.Year == today.Year && a.Time.Month == today.Month)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 4:
                            result = await _repositoryComment.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .Where(a => a.Time.Year == today.Year && a.Time.Month == today.Month)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        default:
                            throw new DataException("نوع مطلب درخواستی نامعتبر است");
                    }

                case 3:
                    switch (type)
                    {
                        case 1:
                            var list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => a.Time >= week)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 2:
                            var result = await _repositoryLike.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            var ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                               .Where(a => !a.VersionStatus.Equals(2))
                               .Where(a => ids.Contains(a.Id))
                               .Where(a => a.Time >= week)
                               .OrderByDescending(a => a.Time)
                               .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            return list;

                        case 3:
                            result = await _repositoryView.TableNoTracking
                               .GroupBy(a => a.PostId)
                               .Select(g => new { g.Key, Count = g.Count() })
                               .OrderByDescending(a => a.Count)
                               .Take(count)
                               .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .Where(a => a.Time >= week)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        case 4:
                            result = await _repositoryComment.TableNoTracking
                                .GroupBy(a => a.PostId)
                                .Select(g => new { g.Key, Count = g.Count() })
                                .OrderByDescending(a => a.Count)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            ids = result.Select(item => item.Key).ToList();

                            list = await TableNoTracking
                                .Where(a => !a.VersionStatus.Equals(2))
                                .Where(a => ids.Contains(a.Id))
                                .Where(a => a.Time >= week)
                                .OrderByDescending(a => a.Time)
                                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                                .Take(count)
                                .ToListAsync(cancellationToken);

                            return list;

                        default:
                            throw new DataException("نوع مطلب درخواستی نامعتبر است");
                    }

                default:
                    throw new DataException("خطا در اطلاعات ورودی");
            }
        }

        public async Task<ApiResult<List<PostShortSelectDto>>> Search(CancellationToken cancellationToken, string str)
        {
            Assert.NotNullArgument(str, "کلمه مورد جستجو نامعتبر است");

            var list = await TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.Title.Contains(str))
                .OrderByDescending(a => a.Time)
                .ProjectTo<PostShortSelectDto>(Mapper.ConfigurationProvider)
                .Take(DefaultTake)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
