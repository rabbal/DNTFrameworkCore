using System;
using System.Linq.Expressions;

namespace DNTFrameworkCore.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> CombineExpressions<T>(this Expression<Func<T, bool>> expression1,
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

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _newValue;
            private readonly Expression _oldValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue) return _newValue;

                return base.Visit(node);
            }
        }   
    }
}