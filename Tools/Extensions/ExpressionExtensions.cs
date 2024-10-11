using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            Expression<Func<T, bool>> combined = Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(
                    left.Body,
                    new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)
                    ), left.Parameters);

            return combined;
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            Expression<Func<T, bool>> combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    left.Body,
                    new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)
                    ), left.Parameters);

            return combined;
        }

        internal class ExpressionParameterReplacer : ExpressionVisitor
        {
            private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
            {
                ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

                for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                { 
                    ParameterReplacements.Add(fromParameters[i], toParameters[i]);
                }
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ParameterReplacements.TryGetValue(node, out var replacement))
                {
                    node = replacement;
                }

                return base.VisitParameter(node);
            }
        }
    }
}
