using System;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class TokenOptions
    {
        public string SigningKey { set; get; }
        public string Issuer { set; get; }
        public string Audience { set; get; }
        public TimeSpan TokenExpiration { set; get; }
        public bool LoginFromSameUserEnabled { set; get; }
        public bool LogoutEverywhereEnabled { set; get; }
    }
}