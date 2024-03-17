using GeoDatabase.ORM.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Set
{
    internal class ShapesSet<T> : IEnumerable<T>, IShapesQueryable<T>
        where T : new()
    {
        private GeoDbContext _dbContext;
        private ShapesQueryable<T> _shapesQueryable;

        public ShapesSet(GeoDbContext dbContext)
        {
            _dbContext = dbContext;
            ElementType = typeof(T);
            Provider = _dbContext.ServiceProvider.GetRequiredService<IShapesQueryProvider>();
        }

        internal ShapesQueryable<T> ShapesQueryable
        {
            get
            {
                if (_shapesQueryable == null)
                {
                    _shapesQueryable = new ShapesQueryable<T>(Provider);
                }

                return _shapesQueryable;
            }
        }

        public Type ElementType { get; }
        public IShapesQueryProvider Provider { get; }
        public Expression<Func<T, bool>> Expression => ShapesQueryable.Expression;

        public IEnumerator<T> GetEnumerator()
        {
            return ShapesQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
