using System;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class SignInResult
    {
        private Token _token;

        private SignInResult() { }
        public static SignInResult Ok(Token token) => new SignInResult { Succeeded = true, _token = token };
        public static SignInResult Failed(string message) => new SignInResult { Succeeded = false, Message = message };

        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
        public Token Token => Succeeded ? _token : throw new InvalidOperationException();
    }
}