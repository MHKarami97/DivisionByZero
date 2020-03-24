using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using Models.Base;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;
using Services;
using Services.Security;
using Services.Services;
using System.Data;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ISecurity _security;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly IRepository<Follower> _repositoryFollower;

        public UsersController(IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger, IJwtService jwtService,
            UserManager<User> userManager, IRepository<Follower> repositoryFollower, ISecurity security)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
            _repositoryFollower = repositoryFollower;
            _security = security;
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

                return _mapper.Map<UserReturnDto>(user);
            }

            if (userId != id)
                return NotFound();

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Token([FromForm]TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.Grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new DataException("OAuth flow is not password.");

            //Prevent Brute Force Attack
            await Task.Delay(_security.RandomNumber(1, 2), cancellationToken);

            var user = await _userManager.FindByNameAsync(tokenRequest.Username);
            if (user == null)
            {
                _logger.LogError("نام کاربری اشتباه");

                throw new DataException("نام کاربری یا رمز عبور اشتباه است");
            }

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
            {
                _logger.LogError("قفل بودن کاربر");

                throw new DataException("حساب شما قفل شده است، لطفا کمی بعد تلاش کنید");
            }

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value < DateTimeOffset.Now)
                await _userRepository.DisableLockout(user, cancellationToken);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
            if (!isPasswordValid)
            {
                await _userRepository.LockoutIncrease(user, cancellationToken);

                _logger.LogError("رمز عبور اشتباه");

                throw new DataException("نام کاربری یا رمز عبور اشتباه است");
            }

            await _userRepository.UpdateSecurityStampAsync(user, cancellationToken);

            var jwt = await _jwtService.GenerateAsync(user);

            return new JsonResult(jwt);
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ApiResult<AccessToken>> TokenByBody(TokenRequest tokenBodyRequest, CancellationToken cancellationToken)
        {
            if (!tokenBodyRequest.Grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new DataException("OAuth flow is not password.");

            //Prevent Brute Force Attack
            await Task.Delay(_security.RandomNumber(1, 2), cancellationToken);

            var user = await _userManager.FindByNameAsync(tokenBodyRequest.Username);
            if (user == null)
            {
                _logger.LogError("نام کاربری اشتباه");

                throw new DataException("نام کاربری یا رمز عبور اشتباه است");
            }

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
            {
                _logger.LogError("قفل بودن کاربر");

                throw new DataException("حساب شما قفل شده است، لطفا کمی بعد تلاش کنید");
            }

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value < DateTimeOffset.Now)
                await _userRepository.DisableLockout(user, cancellationToken);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenBodyRequest.Password);
            if (!isPasswordValid)
            {
                await _userRepository.LockoutIncrease(user, cancellationToken);

                _logger.LogError("رمز عبور اشتباه");

                throw new DataException("نام کاربری یا رمز عبور اشتباه است");
            }

            await _userRepository.UpdateSecurityStampAsync(user, cancellationToken);

            var jwt = await _jwtService.GenerateAsync(user);

            return jwt;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> LoginByPhone(LoginDto dto, CancellationToken cancellationToken)
        {
            Assert.NotNullArgument(dto.Phone, "شماره وارد شده نامعتبر است");

            //Prevent Brute Force Attack
            await Task.Delay(_security.RandomNumber(1, 2), cancellationToken);

            if (dto.VerifyCode == 0)
            {
                var user = await _userRepository.GetByPhone(dto.Phone, cancellationToken);

                if (user == null)
                {
                    _logger.LogError("شماره موبایل اشتباه");

                    throw new DataException("شماره موبایل اشتباه است");
                }

                user.VerifyCode = _security.RandomNumber(11111, 99999);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new BadRequestException("خطا در ذخیره اطلاعات");

                //todo send code

                return Ok("کد تایید با موفقیت ارسال شد");
            }
            else
            {
                var user = await _userRepository.GetByPhone(dto.Phone, cancellationToken);

                if (user == null)
                    throw new DataException("شماره موبایل اشتباه است");

                if (!user.VerifyCode.Equals(dto.VerifyCode))
                {
                    await _userRepository.LockoutIncrease(user, cancellationToken);

                    _logger.LogError("کد تایید موبایل اشتباه");

                    throw new BadRequestException("اطلاعات اشتباه است");
                }

                if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
                {
                    _logger.LogError("قفل بودن حساب");

                    throw new DataException("حساب شما قفل شده است، لطفا کمی بعد تلاش کنید");
                }

                if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value < DateTimeOffset.Now)
                    await _userRepository.DisableLockout(user, cancellationToken);

                user.VerifyCode = null;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new BadRequestException("خطا در ذخیره اطلاعات");

                await _userRepository.UpdateSecurityStampAsync(user, cancellationToken);

                var jwt = await _jwtService.GenerateAsync(user);

                return new JsonResult(jwt);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromForm]TokenRequest tokenRequest, [FromBody]TokenRequest tokenBodyRequest)
        {
            if (tokenBodyRequest != null)
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

            var user = new User
            {
                Birthday = userDto.Birthday,
                FullName = userDto.FullName.ToLower(),
                Gender = userDto.Gender,
                UserName = userDto.UserName.ToLower(),
                Email = _security.EmailChecker(userDto.Email),
                PhoneNumber = userDto.PhoneNumber.Fa2En()
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
        public virtual async Task<ApiResult> Update(int id, UserUpdateDto user, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var requestedUser = await _userManager.FindByIdAsync(userId.ToString());

            var isAdmin = await _userManager.IsInRoleAsync(requestedUser, "Admin");

            if (!isAdmin)
                if (!userId.Equals(id))
                    return BadRequest();

            var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (!string.IsNullOrEmpty(user.UserName))
                updateUser.UserName = user.UserName;

            if (!string.IsNullOrEmpty(user.FullName))
                updateUser.FullName = user.FullName;

            if (!string.IsNullOrEmpty(user.FullName))
                updateUser.FullName = user.FullName;

            if (user.Gender != 0)
                updateUser.Gender = user.Gender;

            if (!string.IsNullOrEmpty(user.Email))
            {
                updateUser.Email = user.Email;
                updateUser.EmailConfirmed = false;
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                updateUser.PhoneNumber = user.PhoneNumber;
                updateUser.PhoneNumberConfirmed = false;
            }

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