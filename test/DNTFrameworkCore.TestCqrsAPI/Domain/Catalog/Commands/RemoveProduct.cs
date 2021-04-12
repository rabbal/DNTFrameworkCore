using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public class RemoveProduct : CommandBase
    {
        public long ProductId { get; }

        [JsonConstructor]
        public RemoveProduct(long productId) => ProductId = productId;
    }
}