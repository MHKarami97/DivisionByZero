using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Filters;

namespace WebFramework.Api
{
    [Authorize]
    [ApiController]
    [ApiResultFilter]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    public class BaseController : ControllerBase
    {
        public const int DefaultTake = 7;
        public  int? UserId;

        public BaseController()
        {
            UserId = HttpContext?.User?.Identity?.GetUserId<int>();
        }

        //public UserRepository UserRepository { get; set; } => property injection
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }
}
