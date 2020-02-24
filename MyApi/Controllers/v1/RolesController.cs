using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Models.Base;
using Models.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Authorize(Policy = "SuperAdminPolicy")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<Role> _roleManager;

        public RolesController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public virtual async Task<ApiResult<List<Role>>> Get(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

            return roles;
        }

        [HttpPost]
        public async Task<ApiResult<Role>> Get(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
                return NotFound();

            return role;
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleDto role)
        {
            var result = await _roleManager.FindByNameAsync(role.Name);

            if (result != null)
                return BadRequest("این نقش موجود است");

            var roleResult = await _roleManager.CreateAsync(new Role
            {
                Name = role.Name,
                Description = role.Description
            });

            if (roleResult.Succeeded)
                return Ok();

            return BadRequest("خطا در برنامه");
        }

        [HttpPost]
        public async Task<ActionResult> Update(RoleDto role)
        {
            var result = await _roleManager.FindByNameAsync(role.Name);

            if (result == null)
                return BadRequest("این نقش ناموجود است");

            result.Name = role.Name;
            result.Description = role.Description;

            var roleResult = await _roleManager.UpdateAsync(result);

            if (roleResult.Succeeded)
                return Ok();

            return BadRequest("خطا در برنامه");
        }
    }
}