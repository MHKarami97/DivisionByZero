using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IRepository<Follower> _repositoryFollower;

        public UsersController(IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger, IJwtService jwtService,
            UserManager<User> userManager, IRepository<Follower> repositoryFollower)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
            _repositoryFollower = repositoryFollower;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdminPolicy")]
        public virtual async Task<ActionResult<List<UserReturnDto>>> Get(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ProjectTo<UserReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<UserReturnDto>> Get(int id, CancellationToken cancellationToken)
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

                return _mapper.Map<UserReturnDto>(user);
            }

            if (userId != id)
                return NotFound();

            await _userManager.UpdateSecurityStampAsync(requestedUser);

            return _mapper.Map<UserReturnDto>(requestedUser);
        }

        [HttpGet]
        public virtual async Task<ApiResult<UserShortReturnDto>> GetByUsername(string username, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            var mapped = _mapper.Map<UserShortReturnDto>(user);

            mapped.IsFollowed = false;

            if (!UserIsAutheticated)
                return mapped;

            var userId = HttpContext.User.Identity.GetUserId<int>();

            var isFollowed = await _repositoryFollower.TableNoTracking
                .AnyAsync(a => a.FollowerId.Equals(user.Id) && a.UserId.Equals(userId), cancellationToken);

            if (isFollowed)
                mapped.IsFollowed = true;

            return mapped;
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdminPolicy")]
        public virtual async Task<ApiResult> IsAdmin(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var requestedUser = await _userManager.FindByIdAsync(userId.ToString());

            var isAdmin = await _userManager.IsInRoleAsync(requestedUser, "Admin");

            if (isAdmin)
                return Ok();

            return BadRequest();
        }

        [HttpGet]
        public virtual async Task<ApiResult<UserReturnDto>> GetUserInfo(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var requestedUser = await _userManager.FindByIdAsync(userId.ToString());

            return _mapper.Map<UserReturnDto>(requestedUser);
        }

        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="tokenBodyRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<ActionResult> Token([FromForm]TokenRequest tokenRequest, [FromBody]TokenRequest tokenBodyRequest, CancellationToken cancellationToken)
        {
            if (tokenBodyRequest != null)
            {
                if (!tokenBodyRequest.Grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("OAuth flow is not password.");

                var user = await _userManager.FindByNameAsync(tokenBodyRequest.Username);
                if (user == null)
                    throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenBodyRequest.Password);
                if (!isPasswordValid)
                    throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

                var jwt = await _jwtService.GenerateAsync(user);

                return new JsonResult(jwt);
            }
            else
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
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromForm]TokenRequest tokenRequest, [FromBody]TokenRequest tokenBodyRequest)
        {
            if(tokenBodyRequest!=null)
            {
                var refreshToken = tokenBodyRequest.Refresh_token;

                if (string.IsNullOrWhiteSpace(refreshToken))
                    return BadRequest("refreshToken is not set.");

                var token = await _jwtService.FindTokenAsync(refreshToken);

                if (token == null)
                    return Unauthorized();

                var jwt = await _jwtService.GenerateAsync(token.User);

                return new JsonResult(jwt);
            }
            else
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
        }

        [HttpGet]
        [AllowAnonymous]
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
        public virtual async Task<ApiResult<UserReturnDto>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            _logger.LogError("متد Create فراخوانی شد");

            var exists = await _userManager.Users.AnyAsync(p => p.PhoneNumber == userDto.PhoneNumber, cancellationToken: cancellationToken);
            if (exists)
                return BadRequest("نام کاربری تکراری است");

            var user = new User
            {
                Birthday = userDto.Birthday,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.ToString());

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
                return BadRequest();

            return _mapper.Map<UserReturnDto>(user);
        }

        [HttpPut]
        public virtual async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var requestedUser = await _userManager.FindByIdAsync(userId.ToString());

            var isAdmin = await _userManager.IsInRoleAsync(requestedUser, "Admin");

            if (!isAdmin)
                if (!userId.Equals(id))
                    return BadRequest();

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