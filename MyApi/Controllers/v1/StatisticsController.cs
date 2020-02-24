using Models.Models;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.Post;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Base;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class StatisticsController : BaseController
    {
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<View> _repositoryView;
        private readonly IRepository<Post> _repositoryPost;
        private readonly IRepository<User> _repositoryUser;

        public StatisticsController(IRepository<Comment> repositoryComment, IRepository<View> repositoryView, IRepository<Post> repositoryPost, IRepository<User> repositoryUser)
        {
            _repositoryComment = repositoryComment;
            _repositoryView = repositoryView;
            _repositoryPost = repositoryPost;
            _repositoryUser = repositoryUser;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<StatisticDto>> Get(CancellationToken cancellationToken)
        {
            var countPost = await _repositoryPost
                .TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2), cancellationToken);

            var countComment = await _repositoryComment
                .TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2), cancellationToken);

            var countView = await _repositoryView
                .TableNoTracking
                .CountAsync(a => !a.VersionStatus.Equals(2), cancellationToken);

            var countUser = await _repositoryUser
                .TableNoTracking
                .CountAsync(a => a.IsActive.Equals(true), cancellationToken);

            return new StatisticDto
            {
                Users = countUser,
                Comments = countComment,
                Posts = countPost,
                Views = countView
            };
        }
    }
}