using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using Models.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class FavoritesController : CrudController<FavoriteSelectDto, FavoriteDto, Favorite>
    {
        public FavoritesController(IRepository<Favorite> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        public override async Task<ApiResult<List<FavoriteDto>>> Get(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId))
                .ProjectTo<FavoriteDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        public override async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var isValid = await Repository.TableNoTracking.AnyAsync(a => a.UserId.Equals(userId), cancellationToken);

            if (isValid)
                return await base.Delete(id, cancellationToken);

            return BadRequest();
        }

        public override Task<ApiResult<FavoriteDto>> Create(FavoriteSelectDto dto, CancellationToken cancellationToken)
        {
            dto.UserId = HttpContext.User.Identity.GetUserId<int>();

            return base.Create(dto, cancellationToken);
        }

        [NonAction]
        public override Task<ApiResult<FavoriteDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [NonAction]
        public override Task<ApiResult<FavoriteDto>> Update(int id, FavoriteSelectDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }
    }
}