using System;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class SignInResult
    {
        private Token _token;

        private SignInResult() { }
        public static SignInResult Ok(Token token) => new SignInResult { Failed = false, _token = token };
        public static SignInResult Fail(string message) => new SignInResult { Failed = true, Message = message };

        public bool Failed { get; private set; }
        public string Message { get; private set; }
        public Token Token => !Failed ? _token : throw new InvalidOperationException();
    }
}