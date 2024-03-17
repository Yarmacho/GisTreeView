using System;
using System.Linq.Expressions;
using Tools.Extensions;

namespace GeoDatabase.ORM.Set.Extensions
{
    public static class ShapesSetExtensions
    {
        public static IShapesQueryable<T> Where<T>(this IShapesQueryable<T> queryable, Expression<Func<T, bool>> predicate)
            where T : new()
        {
            return queryable.Provider.CreateQuery<T>(queryable.Expression.AndAlso(predicate));
        }
    }
}
