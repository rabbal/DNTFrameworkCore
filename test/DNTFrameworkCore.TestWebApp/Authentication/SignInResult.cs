using System;

namespace DNTFrameworkCore.TestWebApp.Authentication
{
    public class SignInResult
    {
        private SignInResult() { }
        public static SignInResult Ok() => new SignInResult { Succeeded = true };
        public static SignInResult Failed(string message) => new SignInResult { Succeeded = false, Message = message };

        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
    }
}