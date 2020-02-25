using AutoMapper;
using Common;
using Common.Exceptions;
using Common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;

namespace Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository, IScopedDependency
    {
        public UserRepository(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(password);

            return Table.Where(p => p.UserName == username && p.PasswordHash == passwordHash).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<User> GetByPhone(string phone, CancellationToken cancellationToken)
        {
            return Table.Where(p => p.PhoneNumber == phone).SingleOrDefaultAsync(cancellationToken);
        }

        public Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public override void Update(User entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            base.Update(entity, saveNow);
        }

        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;
            return UpdateAsync(user, cancellationToken);
        }

        public Task LockoutIncrease(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount += 1;

            if (user.AccessFailedCount <= 3)
                return UpdateAsync(user, cancellationToken);

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.Now.AddHours(2);

            return UpdateAsync(user, cancellationToken);
        }

        public Task DisableLockout(User user, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            return UpdateAsync(user, cancellationToken);
        }

        public Task ChangeUserLockout(User user, bool status, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = status;
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public Task ChangeUserStatus(User user, bool status, CancellationToken cancellationToken)
        {
            user.IsActive = status;
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public Task ChangeUserTwoFactorAuthenticationStatus(User user, bool status, CancellationToken cancellationToken)
        {
            user.IsActive = status;
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public Task EndUserLockout(User user, CancellationToken cancellationToken)
        {
            user.LockoutEnd = null;
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public Task ActivateUserEmail(User user, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString();

            return UpdateAsync(user, cancellationToken);
        }

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName, cancellationToken);
            if (exists)
                throw new BadRequestException("نام کاربری تکراری است");

            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.PasswordHash = passwordHash;
            await base.AddAsync(user, cancellationToken);
        }
    }
}
