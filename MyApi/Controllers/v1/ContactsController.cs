using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Utilities;
using Models.Models;
using Data.Contracts;
using Entities.Contact;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Base;
using Repositories.Contracts;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class ContactsController : CrudController<ContactDto, ContactSelectDto, Contact>
    {
        private readonly UserManager<User> _userManager;
        private readonly IContactRepository _contactRepository;

        public ContactsController(IRepository<Contact> repository, IMapper mapper, UserManager<User> userManager, IContactRepository contactRepository)
            : base(repository, mapper)
        {
            _userManager = userManager;
            _contactRepository = contactRepository;
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
                return await _contactRepository.Get(cancellationToken);

            return await _contactRepository.Get(cancellationToken, user.Id);
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
            var user = await _userManager.GetUserAsync(HttpContext.User);

            dto.ByServer = false;

            //dto.Time = DateTimeOffset.Now;
            //dto.Text = dto.Text.FixPersianChars();

            dto.UserId = HttpContext.User.Identity.GetUserId<int>();

            if (await _userManager.IsInRoleAsync(user, "Member"))
                return await base.Create(dto, cancellationToken);

            if (dto.ParentContactId == 0)
                return BadRequest("فقط کاربر عادی می تواند تیکت ایجاد کند");

            dto.ByServer = true;

            return await base.Create(dto, cancellationToken);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<ContactSelectDto>>> GetByUserId(int id, CancellationToken cancellationToken)
        {
            return await _contactRepository.GetByUserId(id, cancellationToken);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<List<ContactSelectDto>>> GetByParentId(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!await _userManager.IsInRoleAsync(user, "Member"))

                return await _contactRepository.GetByParentId(cancellationToken, id);

            return await _contactRepository.GetByParentId(cancellationToken, id, user.Id);
        }
    }
}