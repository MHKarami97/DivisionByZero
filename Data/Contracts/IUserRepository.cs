using System.Threading;
using System.Threading.Tasks;
using Entities.User;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken);

        Task AddAsync(User user, string password, CancellationToken cancellationToken);
        Task<User> GetByPhone(string phone, CancellationToken cancellationToken);
        Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);
        Task LockoutIncrease(User user, CancellationToken cancellationToken);
        Task DisableLockout(User user, CancellationToken cancellationToken);
        Task ActivateUserEmail(User user, CancellationToken cancellationToken);
        Task EndUserLockout(User user, CancellationToken cancellationToken);
        Task ChangeUserStatus(User user, bool status, CancellationToken cancellationToken);
        Task ChangeUserLockout(User user, bool status, CancellationToken cancellationToken);
        Task ChangeUserTwoFactorAuthenticationStatus(User user, bool status, CancellationToken cancellationToken);
    }
}