using MapWinGIS;
using System.Collections.Generic;

namespace GeoDatabase.ORM.Set
{
    public interface IShapesSet<T> : IEnumerable<T>, IShapesQueryable<T>
        where T : new()
    {
        EntityEntry<T> Add(T entity, Shape shape = null);
        EntityEntry<T> Update(T entity, Shape shape = null);
        EntityEntry<T> Delete(T entity);
    }
}
