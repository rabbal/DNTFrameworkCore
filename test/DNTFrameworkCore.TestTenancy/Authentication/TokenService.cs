using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestTenancy.Domain.Identity;

namespace DNTFrameworkCore.TestTenancy.Authentication
{
     public interface ITokenService : IScopedDependency
    {
        Task<bool> IsValidTokenAsync(long userId, string token);
        Task<Token> NewTokenAsync(long userId, IEnumerable<Claim> claims);
        Task RevokeTokensAsync(long? userId = null);
    }

    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _uow;
        private readonly IOptionsSnapshot<TokenOptions> _options;
        private readonly IDateTime _dateTime;
        private readonly ISecurityService _security;
        private readonly DbSet<UserToken> _tokens;

        public TokenService(IUnitOfWork uow,
            IOptionsSnapshot<TokenOptions> options,
            IDateTime dateTime,
            ISecurityService security)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
            _security = security ?? throw new ArgumentNullException(nameof(security));

            _tokens = _uow.Set<UserToken>();
        }

        public async Task<Token> NewTokenAsync(long userId, IEnumerable<Claim> claims)
        {
            var token = Token.New(_options.Value, claims);

            await AddUserTokenAsync(userId, token.Value);

            return token;
        }

        public async Task RevokeTokensAsync(long? userId)
        {
            if (userId.HasValue)
            {
                if (_options.Value.LogoutEverywhereEnabled)
                {
                    await DeleteUserTokensAsync(userId.Value);
                }
            }

            await DeleteExpiredTokensAsync();

            await _uow.SaveChangesAsync();
        }

        public async Task<bool> IsValidTokenAsync(long userId, string token)
        {
            var tokenHash = ComputeHash(token);

            var userToken = await _tokens.AsNoTracking().FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UserId == userId);

            return userToken?.TokenExpirationDateTime >= _dateTime.UtcNow;
        }

        private async Task AddUserTokenAsync(UserToken token)
        {
            if (!_options.Value.LoginFromSameUserEnabled)
            {
                await DeleteUserTokensAsync(token.UserId);
            }

            _tokens.Add(token);

            await _uow.SaveChangesAsync();
        }

        private async Task AddUserTokenAsync(long userId, string token)
        {
            var now = _dateTime.UtcNow;
            var userToken = new UserToken
            {
                UserId = userId,
                TokenHash = ComputeHash(token),
                TokenExpirationDateTime = now.Add(_options.Value.TokenExpiration)
            };

            await AddUserTokenAsync(userToken);
        }

        private async Task DeleteExpiredTokensAsync()
        {
            var now = _dateTime.UtcNow;
            await _tokens.Where(x => x.TokenExpirationDateTime < now)
                .ForEachAsync(userToken => { _tokens.Remove(userToken); });
        }


        private async Task DeleteUserTokensAsync(long userId)
        {
            await _tokens.Where(x => x.UserId == userId)
                .ForEachAsync(userToken => { _tokens.Remove(userToken); });
        }

        private string ComputeHash(string input)
        {
            return _security.ComputeSha256Hash(input);
        }
    }
}