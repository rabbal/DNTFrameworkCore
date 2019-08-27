using DNTFrameworkCore.Cqrs.Commands;
using Newtonsoft.Json;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class NewPriceType : ICommand
    {
        public string Title { get; }

        [JsonConstructor]
        public NewPriceType(string title) => Title = title;
    }
}