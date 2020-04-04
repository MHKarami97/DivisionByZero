using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class CategoriesController : CrudController<CategoryCreateDto, CategoryDto, Category>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IRepository<Category> repository, IMapper mapper, ICategoryRepository categoryRepository)
            : base(repository, mapper)
        {
            _categoryRepository = categoryRepository;
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
            if (dto.ParentCategoryId == 0 || dto.ParentCategoryId == null)
                return await base.Create(dto, cancellationToken);

            var isParentExist = await Repository.TableNoTracking.SingleOrDefaultAsync(a => a.Id.Equals(dto.ParentCategoryId), cancellationToken);

            if (isParentExist == null)
                return BadRequest("دسته مادر موجود نمی باشد");

            if (!isParentExist.ParentCategoryId.Equals(0) || isParentExist.ParentCategoryId != null)
                return BadRequest("امکان دسته بندی بیشتر از دو مرحله امکان پذیر نمی باشد");

            return await base.Create(dto, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllMainCat(CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllMainCat(cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<List<CategoryWithSubCatDto>>> GetCategoryWithSub(CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetCategoryWithSub(cancellationToken);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<CategoryDto>>> GetAllByCatId(int id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllByCatId(id, cancellationToken);
        }
    }
}