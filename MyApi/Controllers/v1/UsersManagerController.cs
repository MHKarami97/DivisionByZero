using Common.Utilities;
using Data.Contracts;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Entities.User;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;
using Models.Base;
using System.Threading;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UsersManagerController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRepository _userRepository;

        public UsersManagerController(UserManager<User> userManager, RoleManager<Role> roleManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Authorize(Policy = "WorkerPolicy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ActivateUserEmail(int userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            await _userRepository.ActivateUserEmail(user, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "WorkerPolicy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserLockout(int userId, bool status, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            await _userRepository.ChangeUserLockout(user, status, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdminPolicy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserRoles(int userId, int[] roleIds)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var roleList = new string[] { };

            foreach (var roleId in roleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());

                roleList.Append(role.Name);
            }

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.AddToRolesAsync(user, roleList);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "WorkerPolicy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserStatus(int userId, bool status, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            await _userRepository.ChangeUserStatus(user, status, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserTwoFactorAuthenticationStatus(int userId, bool status, CancellationToken cancellationToken)
        {
            var userAuthorizedId = HttpContext.User.Identity.GetUserId<int>();
            var requestedUser = await _userManager.FindByIdAsync(userAuthorizedId.ToString());

            var isAdmin = await _userManager.IsInRoleAsync(requestedUser, "Admin");

            if (isAdmin)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                await _userRepository.ChangeUserTwoFactorAuthenticationStatus(user, status, cancellationToken);

                return Ok();
            }

            await _userRepository.ChangeUserTwoFactorAuthenticationStatus(requestedUser, status, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "WorkerPolicy")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> EndUserLockout(int userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            await _userRepository.EndUserLockout(user, cancellationToken);

            return Ok();
        }
    }
}