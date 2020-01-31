using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class CategoriesController : CrudController<CategoryDto, Category>
    {
        public CategoriesController(IRepository<Category> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [AllowAnonymous]
        public override Task<ApiResult<CategoryDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        [AllowAnonymous]
        public override Task<ApiResult<List<CategoryDto>>> Get(CancellationToken cancellationToken)
        {
            return base.Get(cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<CategoryDto>> Update(int id, CategoryDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllMainCat(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => a.ParentCategoryId.Equals(1))
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllByCatId(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => a.ParentCategoryId.Equals(id))
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
