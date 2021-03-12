using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Handlers
{
    public class RemovePriceTypeHandler : CommandHandler<RemovePriceType>
    {
        private readonly IDbContext _dbContext;

        public RemovePriceTypeHandler(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public override async Task<Result> Handle(RemovePriceType command, CancellationToken cancellationToken)
        {
            var priceType = await _dbContext.Set<PriceType>().FindAsync(command.Id, cancellationToken);
            if (priceType == null) return Fail($"priceType with id:{command.Id} not found");

            _dbContext.Set<PriceType>().Remove(priceType);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}