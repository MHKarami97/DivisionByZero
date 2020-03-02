using AutoMapper;
using Common;
using Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace Models.Models
{
    public class UserDto : IValidatableObject
    {
        private readonly SiteSettings _settings;

        public UserDto(SiteSettings settings)
        {
            _settings = settings;
        }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (_settings.UsernameBanList.Contains(UserName))
                yield return new ValidationResult("نام کاربری نمیتواند مقدار وارد شده باشد", new[] { nameof(UserName) });

            if (_settings.PasswordsBanList.Contains(Password))
                yield return new ValidationResult("رمز عبور نمیتواند مقدرا وارد شده باشد", new[] { nameof(Password) });

            var isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            var isPhone = Regex.IsMatch(PhoneNumber, @"^(\+98|0)?9\d{9}$", RegexOptions.IgnoreCase);

            if (!isEmail)
                yield return new ValidationResult("ایمیل نامعتبر است", new[] { nameof(Email) });

            if (!isPhone)
                yield return new ValidationResult("موبایل نامعتبر است", new[] { nameof(PhoneNumber) });

            if (_settings.EmailsBanList.Contains(Email.Split('@')[1]))
                yield return new ValidationResult("ایمیل در بن لیست قرار دارد", new[] { nameof(Email) });
        }
    }

    public class UserUpdateDto : IValidatableObject
    {
        private readonly SiteSettings _settings;

        public UserUpdateDto(SiteSettings settings)
        {
            _settings = settings;
        }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime Birthday { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(UserName) && _settings.UsernameBanList.Contains(UserName))
                yield return new ValidationResult("نام کاربری نمیتواند مقدار وارد شده باشد", new[] { nameof(UserName) });

            var isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            var isPhone = Regex.IsMatch(PhoneNumber, @"^(\+98|0)?9\d{9}$", RegexOptions.IgnoreCase);

            if (!string.IsNullOrEmpty(Email) && !isEmail)
                yield return new ValidationResult("ایمیل نامعتبر است", new[] { nameof(Email) });

            if (!string.IsNullOrEmpty(PhoneNumber) && !isPhone)
                yield return new ValidationResult("موبایل نامعتبر است", new[] { nameof(PhoneNumber) });

            if (!string.IsNullOrEmpty(Email) && _settings.EmailsBanList.Contains(Email.Split('@')[1]))
                yield return new ValidationResult("ایمیل در بن لیست قرار دارد", new[] { nameof(Email) });
        }
    }

    public class UserReturnDto : IValidatableObject
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Birthday { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
        }
    }

    public class UserShortReturnDto : IValidatableObject
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Birthday { get; set; }

        public GenderType Gender { get; set; }

        [IgnoreMap]
        public bool IsFollowed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
        }
    }
}
