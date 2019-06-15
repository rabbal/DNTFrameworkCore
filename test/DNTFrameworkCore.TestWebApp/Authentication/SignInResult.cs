namespace DNTFrameworkCore.TestWebApp.Authentication
{
    public class SignInResult
    {
        private SignInResult() { }
        public static SignInResult Ok() => new SignInResult { Failed = false };
        public static SignInResult Fail(string message) => new SignInResult { Failed = true, Message = message };

        public bool Failed { get; private set; }
        public string Message { get; private set; }
    }
}