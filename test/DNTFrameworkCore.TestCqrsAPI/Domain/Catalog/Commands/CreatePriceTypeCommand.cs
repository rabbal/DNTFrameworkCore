using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class CreatePriceTypeCommand : ICommand
    {
        public string Title { get; }
        [JsonConstructor]
        public CreatePriceTypeCommand(string title) => Title = title;
    }
}