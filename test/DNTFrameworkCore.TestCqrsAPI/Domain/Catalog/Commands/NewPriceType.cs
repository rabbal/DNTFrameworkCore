using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class NewPriceType : ICommand
    {
        public string Title { get; }
        public NewPriceType(string title) => Title = title;
    }
}