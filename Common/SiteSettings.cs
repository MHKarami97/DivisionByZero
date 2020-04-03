using System.Collections.Generic;

namespace Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public int SecondLevelCache { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public Identity Identity { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public List<string> PasswordsBanList { get; set; }
        public List<string> EmailsBanList { get; set; }
        public List<string> UsernameBanList { get; set; }
    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
    public class Identity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Roles { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
        public bool AllowMultipleLoginsFromTheSameUser { get; set; }
    }
}
