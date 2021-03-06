﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Models.Base;
using Models.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class TwoFactorController : BaseController
    {
        //private readonly IEmailSender _emailSender;
        private readonly ILogger<TwoFactorController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public TwoFactorController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            //IEmailSender emailSender,
            ILogger<TwoFactorController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult> SendCode()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                return NotFound();

            const string tokenProvider = "Email";

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);

            if (string.IsNullOrWhiteSpace(code))
                return BadRequest();

            // await _emailSender.SendEmailAsync(
            //                    user.Email,
            //                    "کد جدید اعتبارسنجی دو مرحله‌ای",
            //                    "~/Views/EmailTemplates/_TwoFactorSendCode.cshtml",
            //                    new TwoFactorSendCodeViewModel
            //                    {
            //                        Token = code,
            //                        EmailSignature = _siteOptions.Value.Smtp.FromName,
            //                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
            //                    });

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> VerifyCode(VerifyDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _signInManager.TwoFactorSignInAsync(
                model.Provider,
                model.Code,
                model.RememberMe,
                model.RememberBrowser);

            if (result.Succeeded)
                Ok();

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");

                return BadRequest("Locked out");
            }

            ModelState.AddModelError(string.Empty, "کد وارد شده غیر معتبر است.");

            return BadRequest("Not valid code");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(int userId, int? verifyCode)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (verifyCode != 0)
            {
                //todo send code

                return Ok();
            }

            if (!user.VerifyCode.Equals(verifyCode))
                return BadRequest();

            user.PhoneNumberConfirmed = true;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}