using GeoDatabase.ORM.Set;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.QueryBuilder
{
    public interface IShapesQueryProvider
    {
        IShapesQueryable<TElement> CreateQuery<TElement>(Expression<Func<TElement, bool>> expression) where TElement : new();
        IEnumerable<TResult> Execute<TResult>(Expression<Func<TResult, bool>> expression) where TResult : new();
    }
}
