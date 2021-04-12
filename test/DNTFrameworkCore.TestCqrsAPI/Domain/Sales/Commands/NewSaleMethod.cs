using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Commands
{
    public class NewSaleMethod : CommandBase
    {
        public string Title { get; set; }
        public int NatureId { get; set; }
    }
}