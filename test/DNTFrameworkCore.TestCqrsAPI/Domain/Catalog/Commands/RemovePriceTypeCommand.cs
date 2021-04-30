using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public sealed class RemovePriceTypeCommand : ICommand
    {
        public int PriceTypeId { get;  }
        [JsonConstructor]
        public RemovePriceTypeCommand(int priceTypeId) => PriceTypeId = priceTypeId;
    }
}