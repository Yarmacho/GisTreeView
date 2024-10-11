using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.QueryBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Set
{
    internal class ShapesQueryable<T> : IShapesQueryable<T>
         where T : new()
    {
        public Expression<Func<T, bool>> Expression { get; }

        public ShapesQueryable(Expression<Func<T, bool>> expression, IShapesQueryProvider provider)
            : this(provider)
        {
            Expression = expression;
        }

        public ShapesQueryable(IShapesQueryProvider provider)
        {
            Provider = provider;
            ElementType = typeof(T);
        }

        public Type ElementType { get; }
        public IShapesQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<T>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
