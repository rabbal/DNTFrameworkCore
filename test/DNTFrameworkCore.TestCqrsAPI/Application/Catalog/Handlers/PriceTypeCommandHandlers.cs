using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Persistence;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class PriceTypeCommandHandlersBase : CommandHandlerBase<RemovePriceType>, ICommandHandler<NewPriceType>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPriceTypeRepository _repository;
        private readonly IPriceTypePolicy _policy;
        private readonly IEventBus _bus;

        public PriceTypeCommandHandlersBase(IUnitOfWork uow, IPriceTypeRepository repository,
            IPriceTypePolicy policy, IEventBus bus)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public override async Task<Result> Handle(RemovePriceType command, CancellationToken cancellationToken)
        {
            var priceType = await _repository.FindAsync(command.PriceTypeId, cancellationToken);
            if (!priceType.HasValue) return Fail($"PriceType with id:{command.Id} not found");

            _repository.Remove(priceType.Value);

            await _uow.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        public async Task<Result> Handle(NewPriceType command, CancellationToken cancellationToken)
        {
            var titleResult = Title.New(command.Title);
            if (titleResult.Failed) return titleResult;

            var title = titleResult.Value;

            var priceTypeResult = PriceType.New(title, _policy);

            if (priceTypeResult.Failed) return priceTypeResult;

            var priceType = priceTypeResult.Value;
            await _repository.AddAsync(priceType, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            await _bus.PublishAsync(priceType);

            return Ok();
        }
    }
}