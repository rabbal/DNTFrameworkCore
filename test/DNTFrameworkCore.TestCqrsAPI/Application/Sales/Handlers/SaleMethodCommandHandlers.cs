using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Persistence;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Repositories;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Sales.Handlers
{
    public class SaleMethodCommandHandlers : CommandHandlerBase<NewSaleMethod>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISaleMethodRepository _repository;

        public SaleMethodCommandHandlers(IUnitOfWork uow, ISaleMethodRepository repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public override async Task<Result> Handle(NewSaleMethod command, CancellationToken cancellationToken)
        {
            var title = Title.New(command.Title);
            if (title.Failed) return Fail(title.Message, title.Failures);

            var saleMethod = SaleMethod.Create(title.Value, SaleNature.FastFood);

            if (saleMethod.Failed) return Fail(saleMethod.Message, saleMethod.Failures);

            await _repository.AddAsync(saleMethod.Value);
            await _uow.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}