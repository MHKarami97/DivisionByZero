using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Base;
using Models.Models;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class LikesController : BaseController
    {
        private readonly ILikeRepository _likeRepository;

        public LikesController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> LikePost(int id, float rate, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            if (await _likeRepository.LikePost(userId, id, rate, cancellationToken))
                return Ok("با موفقیت انجام شد");

            return BadRequest("خطا در برنامه");
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> DisLikePost(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            if (await _likeRepository.DisLikePost(userId, id, cancellationToken))
                return Ok("با موفقیت انجام شد");

            return BadRequest("خطا در برنامه");
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<LikeDto>>> Get(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();

            return await _likeRepository.Get(userId, cancellationToken);
        }
    }
}