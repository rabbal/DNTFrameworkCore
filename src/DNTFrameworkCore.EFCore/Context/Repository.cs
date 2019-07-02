using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Data;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context
{
    //Under Development
    public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : AggregateRoot<TKey>
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IUnitOfWork UnitOfWork;

        protected Repository(IUnitOfWork uow)
        {
            UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
            EntitySet = UnitOfWork.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> BuildFindQuery()
        {
            return EntitySet.AsNoTracking();
        }

        public Task<Maybe<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Maybe<TEntity>> FindAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TEntity>> FindListAsync(IEnumerable<TKey> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedQueryResult<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<TEntity> entityList)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(IEnumerable<TEntity> entityList)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(IEnumerable<TEntity> entityList)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IEnumerable<TEntity> entityList)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IEnumerable<TKey> ids)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Predicate<TEntity>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}