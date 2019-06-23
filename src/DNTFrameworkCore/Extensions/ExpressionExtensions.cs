using System;
using System.Linq.Expressions;
using DNTFrameworkCore.Linq;

namespace DNTFrameworkCore.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> expression1,
            Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left ?? throw new InvalidOperationException(nameof(left)),
                    right ?? throw new InvalidOperationException(nameof(right))), parameter);
        }
    }
}