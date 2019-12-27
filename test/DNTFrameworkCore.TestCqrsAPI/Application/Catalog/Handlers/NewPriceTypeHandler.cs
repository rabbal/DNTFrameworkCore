using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class NewPriceTypeHandler : CommandHandler<NewPriceType>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPriceTypePolicy _policy;
        private readonly IEventBus _bus;

        public NewPriceTypeHandler(IUnitOfWork uow, IPriceTypePolicy policy, IEventBus bus)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public override async Task<Result> Handle(NewPriceType command, CancellationToken cancellationToken)
        {
            var titleResult = Title.New(command.Title);
            if (titleResult.Failed) return Fail(titleResult.Message);

            var title = titleResult.Value;

            var priceTypeResult = PriceType.New(title, _policy);

            if (priceTypeResult.Failed) return Fail(priceTypeResult.Message);

            var priceType = priceTypeResult.Value;
            _uow.Set<PriceType>().Add(priceType);

            await _uow.SaveChangesAsync(cancellationToken);

            await _bus.PublishAsync(priceType);

            return Ok();
        }
    }
}