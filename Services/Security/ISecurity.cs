using Microsoft.AspNetCore.Http;

namespace Services.Security
{
    public interface ISecurity
    {
        string RemoveDangerousString(string input);

        string RemoveScript(string input);

        string GetUniqueFileName(string input);

        int ImageCheck(IFormFile file);
    }
}