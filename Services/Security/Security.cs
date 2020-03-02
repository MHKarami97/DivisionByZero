using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Services.Security
{
    public class Security : ISecurity
    {
        public string RemoveDangerousString(string input)
        {
            var noHtml = Regex.Replace(input, @"<[^>]+>|&nbsp;", "");

            noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            var noHtmlNormalized = Regex.Replace(noHtml, @"\s{2,}", " ");

            noHtmlNormalized = noHtmlNormalized.Replace("|", "").Replace("'", "").Replace('"', ' ').Replace("&", "")
                .Replace("<", "").Replace(">", "").Replace("(", "").Replace(")", "").Replace("<script>", "").Replace("unoin", "")
                .Replace("where", "").Replace("select", "").Replace("'", "");

            return noHtmlNormalized;
        }

        public string RemoveScript(string input)
        {
            input = input.Replace("<script[^<]*</script>", "");
            return Regex.Replace(input, @"<script[^<]*</script>", string.Empty);
        }

        public string GetUniqueFileName(string input)
        {
            input = Path.GetFileName(input).Replace(' ', '-');

            return Path.GetFileNameWithoutExtension(input)
                   + "-"
                   + Guid.NewGuid()
                   + Path.GetExtension(input);
        }

        public int ImageCheck(IFormFile file)
        {
            if (file == null || file.Length < 0 || Path.GetExtension(file.FileName) == null)
                return 1;

            if (file.Length >= 900000)
                return 3;

            if (!IsImage(file))
                return 2;

            return 0;
        }

        private static bool IsImage(IFormFile postedFile)
        {
            const int imageMinimumBytes = 1;

            // Check the image mime types
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/png")
                return false;

            // Check the image extension
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
                return false;

            // Attempt to read the file and check the first bytes
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                    return false;

                // Check whether the image size exceeding the limit or not
                if (postedFile.Length < imageMinimumBytes)
                    return false;

                var buffer = new byte[imageMinimumBytes];

                postedFile.OpenReadStream().Read(buffer, 0, imageMinimumBytes);

                var content = System.Text.Encoding.UTF8.GetString(buffer);

                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            try
            {
                using var bitmap = new System.Drawing.Bitmap(postedFile.OpenReadStream());
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }

        public int RandomNumber(int min = 0, int max = int.MaxValue)
        {
            return RandomNumberGenerator.GetInt32(min, max);
        }

        public bool TimeCheck(DateTimeOffset time)
        {
            return DateTimeOffset.Now >= time.AddMinutes(10);
        }

        public string EmailChecker(string email)
        {
            var emailCheck = email.Split('@');
            emailCheck[0] = emailCheck[0].Replace(".", "");

            return emailCheck.ToString().ToLower();
        }
    }
}