using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class Token
    {
        [JsonPropertyName("token")] public string Value { get; private set; }
        private Token(string value)
        {
            Value = value;
        }

        public static Token New(TokenOptions options, IEnumerable<Claim> claims)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;

            var securityToken = new JwtSecurityToken(
                options.Issuer,
                options.Audience,
                claims,
                now,
                now.Add(options.TokenExpiration),
                credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            
            return new Token(token);
        }

        public override string ToString()
        {
            return Value;
        }
    }

}