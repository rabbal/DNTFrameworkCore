using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class RemovePriceTypeHandler : CommandHandler<RemovePriceType>
    {
        private readonly IUnitOfWork _uow;

        public RemovePriceTypeHandler(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public override async Task<Result> Handle(RemovePriceType command, CancellationToken cancellationToken)
        {
            var priceType = await _uow.Set<PriceType>().FindAsync(command.Id, cancellationToken);
            if (priceType == null) return Fail($"priceType with id:{command.Id} not found");

            _uow.Set<PriceType>().Remove(priceType);

            await _uow.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}