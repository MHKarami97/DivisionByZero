using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Models.Base;
using Services.Security;
using System.IO;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class FilesController : BaseController
    {
        private readonly ISecurity _security;
        private readonly IHostEnvironment _environment;

        public FilesController(ISecurity security, IHostEnvironment environment)
        {
            _security = security;
            _environment = environment;
        }

        [HttpPost]
        [RequestSizeLimit(900_000)]
        public ApiResult<string> UploadImage(IFormFile file)
        {
            switch (_security.ImageCheck(file))
            {
                case 0:
                    break;

                case 1:
                    return BadRequest("فایل نامعتبر است");

                case 2:
                    return BadRequest("فرمت فایل نامعتبر است");

                case 3:
                    return BadRequest("حداکثر حجم فایل نامعتبر است");
            }

            var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");

            var address = _security.GetUniqueFileName(file.FileName);

            var fullPath = Path.Combine(uploads, address);

            file.CopyTo(new FileStream(fullPath, FileMode.Create));

            return Ok("uploads/" + address);
        }
    }
}