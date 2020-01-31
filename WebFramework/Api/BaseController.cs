﻿using Microsoft.AspNetCore.Authorization;
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
        //public UserRepository UserRepository { get; set; } => property injection
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }
}
