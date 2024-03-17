using MapWinGIS;
using System;

namespace GeoDatabase.ORM
{
    public abstract class EntityEntry
    {
        public int ShapeIndex { get; }
        public EntityState State { get; set; }
        public Shape Shape { get; set; }
        public object Entity { get; }
        public Type EntityType {get;set;}

        protected EntityEntry(object entity, int shapeIndex)
        {
            Entity = entity;
            EntityType = entity.GetType();
            ShapeIndex = shapeIndex;
        }
    }

    public class EntityEntry<T> : EntityEntry
    {
        public new T Entity { get; }

        public EntityEntry(T entity, int shapeIndex) : base(entity, shapeIndex)
        {
            Entity = entity;
        }
    }
}
