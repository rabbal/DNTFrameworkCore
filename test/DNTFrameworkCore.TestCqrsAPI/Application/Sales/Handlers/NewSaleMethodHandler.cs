using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Data;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Sales.Handlers
{
    public class NewSaleMethodHandler : CommandHandler<NewSaleMethod>
    {
        private readonly IRepository<SaleMethod, int> _repository;

        public NewSaleMethodHandler(IRepository<SaleMethod, int> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public override async Task<Result> Handle(NewSaleMethod command, CancellationToken cancellationToken)
        {
            var title = Title.New(command.Title);
            if (title.Failed) return Fail(title.Message, title.Failures);

            var saleMethod = SaleMethod.Create(title.Value, SaleNature.FastFood);

            if (saleMethod.Failed) return Fail(saleMethod.Message, saleMethod.Failures);

            await _repository.InsertAsync(saleMethod.Value);

            return Ok();
        }
    }
}