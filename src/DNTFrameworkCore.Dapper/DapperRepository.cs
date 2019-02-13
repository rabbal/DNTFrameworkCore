using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.Dapper
{
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : Entity
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DapperRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public TEntity Single(long id)
        {
            throw new NotImplementedException();
        }

        public TEntity SingleOrDefault(long id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleOrDefaultAsync(long id)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Find(long id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(long id)
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault(long id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync(long id)
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, string sortingProperty,
            bool ascending = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize,
            string sortingProperty,
            bool ascending = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize,
            bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Query(string query, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TAny> Query<TAny>(string query, object parameters = null) where TAny : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters = null) where TAny : class
        {
            throw new NotImplementedException();
        }

        public int Execute(string query, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteAsync(string query, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults,
            string sortingProperty,
            bool ascending = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults,
            bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, string sortingProperty, bool ascending = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            throw new NotImplementedException();
        }

        public void Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public long InsertAndGetId(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<long> InsertAndGetIdAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}