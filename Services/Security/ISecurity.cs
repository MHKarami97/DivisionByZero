using Microsoft.AspNetCore.Http;
using System;

namespace Services.Security
{
    public interface ISecurity
    {
        string RemoveDangerousString(string input);

        string RemoveScript(string input);

        string GetUniqueFileName(string input);

        int ImageCheck(IFormFile file);

        bool TimeCheck(DateTimeOffset time);
    }
}