using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Models.Models;
using Data.Contracts;
using Entities.Contact;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using System;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class ContactsController : CrudController<ContactDto, ContactSelectDto, Contact>
    {
        private readonly UserManager<User> _userManager;

        public ContactsController(IRepository<Contact> repository, IMapper mapper, UserManager<User> userManager)
            : base(repository, mapper)
        {
            _userManager = userManager;
        }

        [NonAction]
        public override Task<ApiResult<ContactSelectDto>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        public override async Task<ApiResult<List<ContactSelectDto>>> Get(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!await _userManager.IsInRoleAsync(user, "Member"))
            {
                var list = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(0))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<ContactSelectDto>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
            else
            {
                var list = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(0) && a.UserId.Equals(user.Id))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<ContactSelectDto>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
        }

        [Authorize(Policy = "WorkerPolicy")]
        public override Task<ApiResult<ContactSelectDto>> Update(int id, ContactDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override async Task<ApiResult<ContactSelectDto>> Create(ContactDto dto, CancellationToken cancellationToken)
        {
            dto.ByServer = false;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            dto.UserId = HttpContext.User.Identity.GetUserId<int>();

            if (!await _userManager.IsInRoleAsync(user, "Member"))
            {
                if (dto.ParentContactId == 0)
                    return BadRequest("فقط کاربر عادی می تواند تیکت ایجاد کند");

                dto.ByServer = true;
            }

            dto.Time = DateTimeOffset.Now;

            return await base.Create(dto, cancellationToken);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<ContactSelectDto>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(id))
                .ProjectTo<ContactSelectDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return list;
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<ContactSelectDto>>> GetByParentId(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!await _userManager.IsInRoleAsync(user, "Member"))
            {
                var list = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(id))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<ContactSelectDto>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
            else
            {
                var list = await Repository.TableNoTracking
                    .Where(a => !a.VersionStatus.Equals(2) && a.ParentContactId.Equals(id) && a.UserId.Equals(user.Id))
                    .OrderByDescending(a => a.Time)
                    .ProjectTo<ContactSelectDto>(Mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return list;
            }
        }
    }
}