namespace DNTFrameworkCore.Validation
{
    public class ValidationFailure
    {
        public ValidationFailure(string memberName, string message)
        {
            MemberName = memberName ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string MemberName { get; }
        public string Message { get; }
    }
}