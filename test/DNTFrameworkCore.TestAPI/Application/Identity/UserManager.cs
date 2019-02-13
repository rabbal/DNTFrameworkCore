using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Identity
{
    public interface IUserManager : ITransientDependency
    {
        Task<Maybe<User>> FindAsync(long userId);
        Task<Maybe<User>> FindIncludeClaimsAsync(long userId);
        Task<Maybe<User>> FindByNameAsync(string name);
        Task UpdateLastActivityDateAsync(User user);
        string NewSerialNumber();
        Task<Maybe<User>> FindCurrentUserAsync();
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }

    public class UserManager : IUserManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserSession _session;
        private readonly IPasswordHasher _hasher;
        private readonly DbSet<User> _users;

        public UserManager(IUnitOfWork uow, IUserSession session, IPasswordHasher hasher)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));

            _users = _uow.Set<User>();
        }

        public async Task<Maybe<User>> FindAsync(long userId)
        {
            return await _users
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == userId);
        }

        public async Task<Maybe<User>> FindIncludeClaimsAsync(long userId)
        {
            return await _users.Include(u => u.Permissions)
                .AsNoTracking()
                .Include(u => u.Claims)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<Maybe<User>> FindByNameAsync(string userName)
        {
            var normalizedUserName = userName.ToUpperInvariant();

            return await _users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName);
        }

        public string NewSerialNumber()
        {
            return Guid.NewGuid().ToString("N");
        }

        public async Task UpdateLastActivityDateAsync(User user)
        {
            if (user.LastLoggedInDateTime.HasValue)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedInDateTime.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }

            user.LastLoggedInDateTime = DateTimeOffset.UtcNow;
            _uow.Entry(user).State = EntityState.Modified;
            await _uow.SaveChangesAsync();
        }

        public Task<Maybe<User>> FindCurrentUserAsync()
        {
            return _session.UserId.HasValue ? FindAsync(_session.UserId.Value) : Task.FromResult(Maybe<User>.None);
        }

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(password);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _hasher.VerifyHashedPassword(hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
        }
    }
}