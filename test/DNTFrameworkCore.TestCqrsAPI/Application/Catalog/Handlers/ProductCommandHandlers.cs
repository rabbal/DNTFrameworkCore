using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Persistence;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class ProductCommandHandlers : CommandHandlerBase<NewProduct>, ICommandHandler<RemoveProduct>
    {
        private readonly IUnitOfWork _uow;
        private readonly IProductRepository _repository;
        private readonly IProductPolicy _policy;
        private readonly IPriceTypeRepository _priceTypeRepository;

        public ProductCommandHandlers(
            IUnitOfWork uow,
            IProductRepository repository,
            IProductPolicy policy,
            IPriceTypeRepository priceTypeRepository)
        {
            _uow = uow;
            _repository = repository;
            _policy = policy;
            _priceTypeRepository = priceTypeRepository;
        }

        public override async Task<Result> Handle(NewProduct command, CancellationToken cancellationToken)
        {
            var title = Title.New(command.Title);
            if (title.Failed) return Fail(title.Message);

            var product = Product.New(title.Value, _policy);
            var priceTypeIds = command.Prices.Select(p => p.PriceTypeId).ToList();
            var priceTypes = (await _priceTypeRepository.FindAsync(p => priceTypeIds.Contains(p.Id), cancellationToken))
                .ToDictionary(p => p.Id);

            if (priceTypes.Count != priceTypeIds.Count) return Fail("Some of priceTypes not found");

            foreach (var priceItem in command.Prices)
            {
                var priceValue = Price.New(priceItem.Price, "$");
                if (priceValue.Failed) return Fail(priceValue.Message);

                product.Value.AddPrice(priceTypes[priceItem.PriceTypeId], priceValue.Value);
            }

            await _uow.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        public async Task<Result> Handle(RemoveProduct command, CancellationToken cancellationToken)
        {
            var product = await _repository.FindAsync(command.ProductId, cancellationToken);
            if (!product.HasValue) return Fail("Product not found!");

            _repository.Remove(product.Value);
            await _uow.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}