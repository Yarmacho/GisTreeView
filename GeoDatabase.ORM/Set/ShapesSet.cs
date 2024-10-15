using GeoDatabase.ORM.QueryBuilder;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Set
{
    internal class ShapesSet<T> : IShapesSet<T>
        where T : new()
    {
        private ShapesQueryable<T> _shapesQueryable;
        private ChangeTracker _changeTracker;

        public ShapesSet(GeoDbContext dbContext, ChangeTracker changeTracker)
        {
            ElementType = typeof(T);
            Provider = dbContext.ServiceProvider.GetRequiredService<IShapesQueryProvider>();
            _changeTracker = changeTracker;
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

        public EntityEntry<T> Add(T entity, Shape shape = null)
        {
            return _changeTracker.AddAdded(entity, shape);
        }

        public EntityEntry<T> Update(T entity, Shape shape = null)
        {
            return _changeTracker.AddUpdated(entity, shape);
        }

        public EntityEntry<T> Delete(T entity)
        {
            return _changeTracker.AddRemoved(entity);
        }

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
