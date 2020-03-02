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

        int RandomNumber(int min = 0, int max = int.MaxValue);

        bool TimeCheck(DateTimeOffset time);

        string EmailChecker(string email);
    }
}