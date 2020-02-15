using Microsoft.AspNetCore.Http;
using System;
using System.IO;
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
                   + "_"
                   + Guid.NewGuid()
                   + Path.GetExtension(input);
        }

        public int ImageCheck(IFormFile file)
        {
            if (file == null || file.Length < 0 || Path.GetExtension(file.FileName) == null)
                return 1;

            if (!file.ContentType.Equals("image/jpeg") && !file.ContentType.Equals("image/png"))
                return 2;

            if (file.Length >= 5000000)
                return 3;

            return 0;
        }
    }
}