using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
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
        private readonly IRepository<View> _repositoryView;
        private readonly IHttpContextAccessor _accessor;
        private readonly ISecurity _security;
        private readonly IMapper _mapper;

        public ViewsController(IRepository<View> repositoryLike, IMapper mapper, ISecurity security, IHttpContextAccessor accessor)
        {
            _repositoryView = repositoryLike;
            _mapper = mapper;
            _security = security;
            _accessor = accessor;
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<ViewDto>>> Get(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            var list = await _repositoryView.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.UserId.Equals(userId))
                .ProjectTo<ViewDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [NonAction]
        public async Task<bool> IncreaseView(int id, bool isAuthorize, CancellationToken cancellationToken)
        {
            int? userId = null;
            var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var lastView = await _repositoryView.TableNoTracking
                .Where(a => !a.VersionStatus.Equals(2) && a.PostId.Equals(id) && a.Ip.Equals(ip))
                .OrderByDescending(a => a.Time)
                .Select(a => a.Time)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastView != DateTimeOffset.MinValue && !_security.TimeCheck(lastView))
                return false;

            if (isAuthorize)
                userId = HttpContext.User.Identity.GetUserId<int>();

            await _repositoryView.AddAsync(new View
            {
                PostId = id,
                UserId = userId,
                Ip = ip,
                Time = DateTimeOffset.Now
            }, cancellationToken);

            return true;
        }
    }
}