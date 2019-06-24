using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public interface ITokenManager : IScopedDependency
    {
        Task<bool> IsValidTokenAsync(long userId, string accessToken);
        Task<Token> BuildTokenAsync(long userId, IEnumerable<Claim> claims);
        Task RevokeTokensAsync(long? userId = null);
    }

    public class TokenManager : ITokenManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IOptionsSnapshot<TokenOptions> _options;
        private readonly ISecurityService _security;
        private readonly DbSet<UserToken> _tokens;

        public TokenManager(IUnitOfWork uow,
            IOptionsSnapshot<TokenOptions> options,
            ISecurityService security)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _security = security ?? throw new ArgumentNullException(nameof(security));

            _tokens = _uow.Set<UserToken>();
        }

        public async Task<Token> BuildTokenAsync(long userId, IEnumerable<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SigningKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var encryptingKey = Encoding.UTF8.GetBytes(_options.Value.EncryptingKey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptingKey),
                SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(_options.Value.TokenExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            await AddUserTokenAsync(userId, token);

            return new Token {Value = token};
        }

        public async Task RevokeTokensAsync(long? userId)
        {
            if (userId.HasValue)
            {
                if (_options.Value.AllowSignOutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId.Value);
                }
            }

            await DeleteExpiredTokensAsync();

            await _uow.SaveChangesAsync();
        }

        public async Task<bool> IsValidTokenAsync(long userId, string token)
        {
            var tokenHash = ComputeHash(token);

            var userToken = await _tokens.FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UserId == userId);

            return userToken?.TokenExpirationDateTime >= DateTimeOffset.UtcNow;
        }

        private async Task AddUserTokenAsync(UserToken userToken)
        {
            if (!_options.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }

            _tokens.Add(userToken);

            await _uow.SaveChangesAsync();
        }

        private async Task AddUserTokenAsync(long userId, string token)
        {
            var now = DateTimeOffset.UtcNow;
            var userToken = new UserToken
            {
                UserId = userId,
                TokenHash = ComputeHash(token),
                TokenExpirationDateTime = now.AddMinutes(_options.Value.TokenExpirationMinutes)
            };

            await AddUserTokenAsync(userToken);
        }

        private async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            await _tokens.Where(x => x.TokenExpirationDateTime < now)
                .ForEachAsync(userToken => { _tokens.Remove(userToken); });
        }

        private async Task InvalidateUserTokensAsync(long userId)
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