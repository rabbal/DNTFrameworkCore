using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class NewPriceType : CommandBase
    {
        public string Title { get; }
        [JsonConstructor]
        public NewPriceType(string title) => Title = title;
    }
}