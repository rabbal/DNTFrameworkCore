using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.Dapper
{
    public interface IDapperRepository<TEntity> where TEntity : Entity
    {
        TEntity Single(long id);
        TEntity SingleOrDefault(long id);
        Task<TEntity> SingleAsync(long id);
        Task<TEntity> SingleOrDefaultAsync(long id);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(long id);
        Task<TEntity> FindAsync(long id);
        TEntity FirstOrDefault(long id);
        Task<TEntity> FirstOrDefaultAsync(long id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, string sortingProperty, bool ascending = true);

        Task<IEnumerable<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression);

        IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, string sortingProperty, bool ascending = true);

        IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber,
            int pageSize, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression);

        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Query(string query, object parameters = null);
        Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters = null);
        IEnumerable<TAny> Query<TAny>(string query, object parameters = null) where TAny : class;

        Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters = null)
            where TAny : class;

        int Execute(string query, object parameters = null);
        Task<int> ExecuteAsync(string query, object parameters = null);

        IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, string sortingProperty, bool ascending = true);

        IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression);

        Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, string sortingProperty, bool ascending = true);

        Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult,
            int maxResults, bool ascending = true,
            params Expression<Func<TEntity, object>>[] sortingExpression);

        void Insert(TEntity entity);
        long InsertAndGetId(TEntity entity);
        Task InsertAsync(TEntity entity);
        Task<long> InsertAndGetIdAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}