using Common.Exceptions;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;
using Services.Services;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IJwtService jwtService,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdminPolicy")]
        public virtual async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var requestedUser = await _userManager.FindByIdAsync(userId.ToString());

            var isAdmin = await _userManager.IsInRoleAsync(requestedUser, "Admin");

            if (isAdmin)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                    return NotFound();

                await _userManager.UpdateSecurityStampAsync(user);

                return user;
            }

            if (userId != id)
                return NotFound();

            await _userManager.UpdateSecurityStampAsync(requestedUser);

            return requestedUser;
        }

        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("[action]")]
        public virtual async Task<ActionResult> Token([FromForm]TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.Grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            var user = await _userManager.FindByNameAsync(tokenRequest.Username);
            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var jwt = await _jwtService.GenerateAsync(user);

            return new JsonResult(jwt);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromForm]TokenRequest tokenRequest)
        {
            var refreshToken = tokenRequest.Refresh_token;

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("refreshToken is not set.");

            var token = await _jwtService.FindTokenAsync(refreshToken);

            if (token == null)
                return Unauthorized();

            var jwt = await _jwtService.GenerateAsync(token.User);

            return new JsonResult(jwt);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<bool> Logout()
        {
            if (!(User.Identity is ClaimsIdentity claimsIdentity))
                return false;

            var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;

            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out var userId))
            {
                await _jwtService.InvalidateUserTokensAsync(userId);
            }

            await _jwtService.DeleteExpiredTokensAsync();

            return true;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            _logger.LogError("متد Create فراخوانی شد");

            var exists = await _userManager.Users.AnyAsync(p => p.PhoneNumber == userDto.PhoneNumber, cancellationToken: cancellationToken);
            if (exists)
                return BadRequest("نام کاربری تکراری است");

            var user = new User
            {
                Birthday = DateTime.Now,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
                return BadRequest();

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
                return BadRequest();

            return user;
        }

        [HttpPut]
        public virtual async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user.UserName;
            updateUser.PasswordHash = user.PasswordHash;
            updateUser.FullName = user.FullName;
            updateUser.Birthday = user.Birthday;
            updateUser.Gender = user.Gender;
            updateUser.IsActive = user.IsActive;
            updateUser.LastLoginDate = user.LastLoginDate;

            await _userRepository.UpdateAsync(updateUser, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = "SuperAdminPolicy")]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user, cancellationToken);

            return Ok();
        }
    }
}