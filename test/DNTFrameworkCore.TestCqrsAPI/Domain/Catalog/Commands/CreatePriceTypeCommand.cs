using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class CreatePriceTypeCommand : CommandBase
    {
        public string Title { get; }
        [JsonConstructor]
        public CreatePriceTypeCommand(string title) => Title = title;
    }
}