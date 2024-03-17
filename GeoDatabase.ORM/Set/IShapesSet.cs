using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoDatabase.ORM.Set
{
    public interface IShapesSet<T> : IEnumerable<T>, IShapesQueryable<T>
        where T : new()
    {
        EntityEntry<T> Add(T entity, Shape shape = null);
        EntityEntry<T> Update(T entity);
        EntityEntry<T> Delete(T entity);
    }
}
