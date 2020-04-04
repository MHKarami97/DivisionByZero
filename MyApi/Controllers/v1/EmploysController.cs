using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Utilities;
using Models.Models;
using Data.Contracts;
using Entities.Employ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Base;
using Repositories.Contracts;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class EmploysController : CrudController<EmployDto, EmploySelectDto, Employ>
    {
        private readonly IEmployRepository _employRepository;

        public EmploysController(IRepository<Employ> repository, IMapper mapper, IEmployRepository employRepository)
            : base(repository, mapper)
        {
            _employRepository = employRepository;
        }

        [AllowAnonymous]
        public override Task<ApiResult<EmploySelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [AllowAnonymous]
        public override async Task<ApiResult<List<EmploySelectDto>>> Get(CancellationToken cancellationToken)
        {
            return await _employRepository.Get(cancellationToken);
        }

        public override Task<ApiResult<EmploySelectDto>> Create(EmployDto dto, CancellationToken cancellationToken)
        {
            dto.UserId = HttpContext.User.Identity.GetUserId<int>();

            //dto.Time = DateTimeOffset.Now;
            //dto.Text = dto.Text.FixPersianChars();
            //dto.Title = dto.Title.FixPersianChars();

            return base.Create(dto, cancellationToken);
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

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<EmploySelectDto>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            return await _employRepository.GetByUserId(id, cancellationToken);
        }
    }
}