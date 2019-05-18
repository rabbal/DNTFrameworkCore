using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Queries;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context;

namespace DNTFrameworkCore.Cqrs.EntityFramework.Queries
{
    public class FindByIdQueryHandler<TKey, TEntity, TReadModel> : IQueryHandler<FindByIdQuery<TKey, TEntity, TReadModel>, TReadModel>
        where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        private readonly IUnitOfWork _uow;

        public FindByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public Task<TReadModel> Handle(FindByIdQuery<TKey, TEntity, TReadModel> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
