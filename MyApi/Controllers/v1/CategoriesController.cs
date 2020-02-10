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
    public class CategoriesController : CrudController<CategoryCreateDto, CategoryDto, Category>
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
        public override Task<ApiResult<CategoryDto>> Update(int id, CategoryCreateDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override async Task<ApiResult<CategoryDto>> Create(CategoryCreateDto dto, CancellationToken cancellationToken)
        {
            if (dto.ParentCategoryId == null)
                dto.ParentCategoryId = 0;

            if (dto.ParentCategoryId == 0)
                return await base.Create(dto, cancellationToken);

            var isParentExist = await Repository.TableNoTracking.AnyAsync(a => a.Id.Equals(dto.ParentCategoryId), cancellationToken);

            if (!isParentExist)
                return BadRequest("دسته مادر موجود نمی باشد");

            return await base.Create(dto, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllMainCat(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(0))
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryWithSubCatDto>>> GetCategoryWithSub(CancellationToken cancellationToken)
        {
            var result = new List<CategoryWithSubCatDto>();

            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(null))
                .ToListAsync(cancellationToken);

            foreach (var category in list)
            {
                var sub = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(category.Id))
                    .ProjectTo<ShortCategoryDto>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                result.Add(new CategoryWithSubCatDto
                {
                    Name = category.Name,
                    Sub = sub
                });
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllByCatId(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.ParentCategoryId.Equals(id))
                .ProjectTo<CategoryDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }
    }
}