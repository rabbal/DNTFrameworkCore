using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public class RemoveProductCommand : CommandBase
    {
        public long ProductId { get; }

        [JsonConstructor]
        public RemoveProductCommand(long productId) => ProductId = productId;
    }
}