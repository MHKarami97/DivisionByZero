using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Entities.User;

namespace MyApi.Models
{
    public class UserDto : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime Birthday { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
            if (Password.Equals("123456"))
                yield return new ValidationResult("رمز عبور نمیتواند 123456 باشد", new[] { nameof(Password) });

            var isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            var isPhone = Regex.IsMatch(Email, @"^(\+98|0)?9\d{9}$", RegexOptions.IgnoreCase);

            if (!isEmail)
                yield return new ValidationResult("ایمیل نامعتبر است", new[] { nameof(Email) });

            if (!isPhone)
                yield return new ValidationResult("موبایل نامعتبر است", new[] { nameof(PhoneNumber) });
        }
    }
}
