using System;
using System.Linq.Expressions;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Extensions
{
    public static class EntityExtensions
    {
        public static Expression<Func<TEntity, bool>> ToEqualityExpression<TEntity, TKey>(this TKey id)
            where TEntity : Entity<TKey>
            where TKey : IEquatable<TKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, nameof(Entity<TKey>.Id)),
                Expression.Constant(id, typeof(TKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}