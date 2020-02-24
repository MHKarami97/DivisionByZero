using Common.Utilities;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ViewsController : BaseController
    {
        private readonly IViewRepository _viewRepository;

        private readonly IRepository<View> _repositoryView;
        private readonly IHttpContextAccessor _accessor;
        private readonly ISecurity _security;

        public ViewsController(IRepository<View> repositoryLike, ISecurity security, IHttpContextAccessor accessor, IViewRepository viewRepository)
        {
            _repositoryView = repositoryLike;
            _security = security;
            _accessor = accessor;
            _viewRepository = viewRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<ViewDto>>> Get(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            return await _viewRepository.Get(userId, cancellationToken);
        }

        [NonAction]
        public async Task<bool> IncreaseView(int id, bool isAuthorize, CancellationToken cancellationToken)
        {
            int? userId = null;

            if (isAuthorize)
                userId = HttpContext.User.Identity.GetUserId<int>();

            var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var lastView = await _repositoryView.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id) && a.Ip.Equals(ip))
                .OrderByDescending(a => a.Time)
                .Select(a => a.Time)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastView != DateTimeOffset.MinValue && !_security.TimeCheck(lastView))
                return false;

            return await _viewRepository.IncreaseView(userId, ip, id, cancellationToken);
        }
    }
}