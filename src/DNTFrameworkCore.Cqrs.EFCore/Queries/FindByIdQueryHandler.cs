using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Queries;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;

namespace DNTFrameworkCore.Cqrs.EFCore.Queries
{
    public class FindByIdQueryHandler<TKey, TEntity, TReadModel> : IQueryHandler<FindByIdQuery<TKey, TEntity, TReadModel>, TReadModel>
        where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        private readonly IUnitOfWork _context;

        public FindByIdQueryHandler(IUnitOfWork context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<TReadModel> Handle(FindByIdQuery<TKey, TEntity, TReadModel> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
