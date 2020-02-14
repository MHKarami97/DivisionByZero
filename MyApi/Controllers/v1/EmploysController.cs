using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using MyApi.Models;
using Data.Contracts;
using Entities.Employ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class EmploysController : CrudController<EmployDto, EmploySelectDto, Employ>
    {
        public EmploysController(IRepository<Employ> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [AllowAnonymous]
        public override Task<ApiResult<EmploySelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [AllowAnonymous]
        public override async Task<ApiResult<List<EmploySelectDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2))
                .OrderByDescending(a => a.Time)
                .ProjectTo<EmploySelectDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<EmploySelectDto>> Update(int id, EmployDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ApiResult<EmploySelectDto>> Create(EmployDto dto, CancellationToken cancellationToken)
        {
            dto.AuthorId = HttpContext.User.Identity.GetUserId<int>();
            dto.Time = DateTime.Now;

            return base.Create(dto, cancellationToken);
        }
    }
}