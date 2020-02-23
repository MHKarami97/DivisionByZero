using Data.Contracts;
using Entities.Post;
using Models.Base;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ICommentRepository<TSelect> : IRepository<Comment>
    {
        Task<ApiResult<List<TSelect>>> GetPostComments(int id, CancellationToken cancellationToken);

        Task<ApiResult<List<TSelect>>> GetLastComments(CancellationToken cancellationToken);

        Task<DateTimeOffset> Create(CommentDto dto, CancellationToken cancellationToken);
    }
}