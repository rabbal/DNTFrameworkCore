using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Validation
{
    public class ValidationFailure
    {
        public ValidationFailure(string memberName, string message)
        {
            MemberName = memberName ?? string.Empty;
            Message = Ensure.IsNotNullOrEmpty(message, nameof(message));
        }

        public string MemberName { get; }
        public string Message { get; }

        public override string ToString()
        {
            return $"[{MemberName}: {Message}]";
        }
    }
}