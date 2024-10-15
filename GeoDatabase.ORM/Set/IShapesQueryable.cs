using GeoDatabase.ORM.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Set
{
    public interface IShapesQueryable : IShapesQueryable<object>
    {
    }

    public interface IShapesQueryable<T> : IEnumerable<T> where T : new()
    {
        IShapesQueryProvider Provider { get; }
        Expression<Func<T, bool>> Expression { get; }
    }
}
