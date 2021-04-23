using System.Text.Json.Serialization;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public class UpdateProductTitleCommand
    {
        public string Title { get; }
        [JsonConstructor]
        public UpdateProductTitleCommand(string title) => Title = title;
    }
}