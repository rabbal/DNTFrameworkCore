using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class OrderNote : Entity
    {
        public const int ContentMaximumLength = 1024;
        public string Content { get; private set; }

        private OrderNote()
        {
        }

        public static Result<OrderNote> Create(string content)
        {
            content = content ?? string.Empty;
            
            if (content.Length == 0)
            {
                return Result.Fail<OrderNote>("content should not be empty");
            }

            if (content.Length > ContentMaximumLength)
            {
                return Result.Fail<OrderNote>("content is too long");
            }
            
            var note = new OrderNote {Content = content};
            
            return Result.Ok(note);
        }
    }
}