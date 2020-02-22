using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Models.Models;
using Data.Contracts;
using Entities.More;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class HelpsController : CrudController<HelpDto, HelpSelectDto, Help>
    {
        public HelpsController(IRepository<Help> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [AllowAnonymous]
        public override Task<ApiResult<HelpSelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [AllowAnonymous]
        public override async Task<ApiResult<List<HelpSelectDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .ProjectTo<HelpSelectDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<HelpSelectDto>> Update(int id, HelpDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<HelpSelectDto>> Create(HelpDto dto, CancellationToken cancellationToken)
        {
            return base.Create(dto, cancellationToken);
        }
    }
}