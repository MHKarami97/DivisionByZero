using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;
using Models.Base;
using Models.Models;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class PasswordsController : BaseController
    {
        //private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IPasswordValidator<User> _passwordValidator;

        public PasswordsController(/*IEmailSender emailSender,*/ IPasswordValidator<User> passwordValidator, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _passwordValidator = passwordValidator;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ApiResult> ChangePassword(ChangePasswordDto model)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var validOldPass = await _passwordValidator.ValidateAsync(_userManager, user, model.OldPassword);

            if (!validOldPass.Succeeded)
                return BadRequest("رمز عبور قبلی معتبر نمی باشد");

            if (!model.NewPassword.Equals(model.ConfirmPassword))
                return BadRequest("رمز عبور و تایید آن برابر نیست");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.UpdateSecurityStampAsync(user);

            //var resultPass = await _passwordValidator.ValidateAsync(_userManager, user, model.OldPassword);

            await _signInManager.RefreshSignInAsync(user);

            // await _emailSender.SendEmailAsync(
            //     user.Email,
            //     "اطلاع رسانی تغییر کلمه‌ی عبور",
            //     "~/Views/EmailTemplates/_ChangePasswordNotification.cshtml",
            //     new ChangePasswordNotificationViewModel
            //     {
            //         User = user,
            //         EmailSignature = _siteOptions.Value.Smtp.FromName,
            //         MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
            //     });

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("کاربر نامعتبر است یا تایید نشده است");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            // await _emailSender.SendEmailAsync(
            //         model.Email,
            //         "بازیابی کلمه‌ی عبور",
            //         "~/Views/EmailTemplates/_PasswordReset.cshtml",
            //         new PasswordResetViewModel
            //         {
            //             UserId = user.Id,
            //             Token = code,
            //             EmailSignature = _siteOptions.Value.Smtp.FromName,
            //             MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
            //         })
            //     ;

            return Ok();
        }
    }
}