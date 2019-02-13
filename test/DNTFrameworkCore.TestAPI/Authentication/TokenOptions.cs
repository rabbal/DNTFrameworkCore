namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class TokenOptions
    {
        public string SigningKey { set; get; }
        public string EncryptingKey { get; set; }
        public string Issuer { set; get; }
        public string Audience { set; get; }
        public int TokenExpirationMinutes { set; get; }
        public bool AllowMultipleLoginsFromTheSameUser { set; get; }
        public bool AllowSignOutAllUserActiveClients { set; get; }
    }
}