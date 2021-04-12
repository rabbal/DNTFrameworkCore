using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class RemovePriceType : CommandBase
    {
        public int PriceTypeId { get;  }
        [JsonConstructor]
        public RemovePriceType(int priceTypeId) => PriceTypeId = priceTypeId;
    }
}