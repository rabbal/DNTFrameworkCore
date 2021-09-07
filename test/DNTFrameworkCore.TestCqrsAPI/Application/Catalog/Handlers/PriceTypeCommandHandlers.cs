using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;
using static DNTFrameworkCore.Functional.Result;
namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class PriceTypeCommandHandlers : ICommandHandler<RemovePriceTypeCommand, Result>,
        ICommandHandler<CreatePriceTypeCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPriceTypeRepository _repository;
        private readonly IPriceTypePolicy _policy;
        private readonly IEventBus _bus;

        public PriceTypeCommandHandlers(
            IUnitOfWork uow,
            IPriceTypeRepository repository,
            IPriceTypePolicy policy,
            IEventBus bus)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task<Result> Handle(RemovePriceTypeCommand command, CancellationToken cancellationToken)
        {
            var priceType = await _repository.FindAsync(command.PriceTypeId, cancellationToken);
            if (priceType is null) return Fail($"PriceType with id:{command.PriceTypeId} not found");

            //Alternative: _repository.Remove(priceType);
            _uow.Set<PriceType>().Remove(priceType);
            
            await _uow.SaveChanges(cancellationToken);
            await _bus.DispatchDomainEvents(priceType, cancellationToken);

            return Ok();
        }

        public async Task<Result> Handle(CreatePriceTypeCommand command, CancellationToken cancellationToken)
        {
            var titleResult = Title.New(command.Title);
            if (titleResult.Failed) return titleResult;

            var title = titleResult.Value;

            var priceTypeResult = PriceType.New(title, _policy);

            if (priceTypeResult.Failed) return priceTypeResult;

            var priceType = priceTypeResult.Value;
            //Alternative: _repository.Add(priceType);
            _uow.Set<PriceType>().Add(priceType);

            await _uow.SaveChanges(cancellationToken);
            await _bus.DispatchDomainEvents(priceType, cancellationToken);

            return Ok();
        }
    }

    public class PriceTypeCommandHandlersV2 : ICommandHandler<RemovePriceTypeCommand, Result>,
       ICommandHandler<CreatePriceTypeCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPriceTypeRepository _repository;
        private readonly IPriceTypePolicy _policy;
        private readonly IEventBus _bus;

        public PriceTypeCommandHandlersV2(
            IUnitOfWork uow,
            IPriceTypeRepository repository,
            IPriceTypePolicy policy,
            IEventBus bus)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task<Result> Handle(RemovePriceTypeCommand command, CancellationToken cancellationToken)
        {
            var priceType = await _repository.FindAsync(command.PriceTypeId, cancellationToken);
            if (priceType is null) return Fail($"PriceType with id:{command.PriceTypeId} not found");
            
            //Alternative: _repository.Remove(priceType);
            _uow.Set<PriceType>().Remove(priceType);
            
            await _uow.SaveChanges(cancellationToken);
            await _bus.DispatchDomainEvents(priceType, cancellationToken);
            
            return Ok();
        }

        public async Task<Result> Handle(CreatePriceTypeCommand command, CancellationToken cancellationToken)
        {
            var title = new Title(command.Title);

            var priceType = new PriceType(title, _policy);

            //Alternative: _repository.Add(priceType);
            _uow.Set<PriceType>().Add(priceType);
            
            await _uow.SaveChanges(cancellationToken);
            await _bus.DispatchDomainEvents(priceType, cancellationToken);

            return None;
        }
    }
}