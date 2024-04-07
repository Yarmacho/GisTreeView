using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoDatabase.ORM
{
    public class ChangeTracker
    {
        private Dictionary<int, EntityEntry> _entries = new Dictionary<int, EntityEntry>();
        private List<EntityEntry> _addEntries = new List<EntityEntry>();

        public bool HasChanges => _entries.Count != 0 || _addEntries.Count != 0;

        public List<EntityEntry> GetUpdated()
        {
            return _entries.Values.Where(e => e.State == EntityState.Updated).ToList();
        }
        public List<EntityEntry> GetAttached()
        {
            return _entries.Values.Where(e => e.State == EntityState.Attached).ToList();
        }
        public List<EntityEntry> GetAdded()
        {
            return _addEntries;
        }
        public List<EntityEntry> GetRemoved()
        {
            return _entries.OrderByDescending(e => e.Key).Where(e => e.Value.State == EntityState.Removed)
                .Select(e => e.Value).ToList();
        }

        public int GetShapeIndex(object entity)
        {
            return _entries.Values.FirstOrDefault(e => e.Entity.Equals(entity))?.ShapeIndex ?? -1;
        }

        public IEnumerable<EntityEntry> GetAllEntries()
        {
            foreach (var value in _addEntries)
            {
                yield return value;
            }

            foreach (var value in _entries.Values.OrderByDescending(e => e.ShapeIndex))
            {
                yield return value;
            }
        }

        internal void Add(EntityEntry entry)
        {
            _entries.Add(entry.ShapeIndex, entry);
        }

        internal EntityEntry<T> Add<T>(T entity, int shapeIndex, EntityState state, Shape shape = null)
        {
            var entry = new EntityEntry<T>(entity, shapeIndex)
            {
                State = state,
                Shape = shape
            };

            if (state == EntityState.Added)
            {
                _addEntries.Add(entry);
            }
            else
            {
                _entries[entry.ShapeIndex] = entry;
            }
            
            return entry;
        }

        internal EntityEntry<T> AddAttached<T>(T entity)
        {
            return Add<T>(entity, getShapeIndex(entity), EntityState.Attached);
        }

        internal EntityEntry<T> AddAttached<T>(T entity, int shapeIndex)
        {
            return Add<T>(entity, shapeIndex, EntityState.Attached);
        }

        internal EntityEntry<T> AddRemoved<T>(T entity)
        {
            return Add<T>(entity, getShapeIndex(entity), EntityState.Removed);
        }

        internal EntityEntry<T> AddAdded<T>(T entity, Shape shape = null)
        {
            return Add<T>(entity, -1, EntityState.Added, shape);
        }

        internal EntityEntry<T> AddUpdated<T>(T entity)
        {
            return Add<T>(entity, getShapeIndex(entity), EntityState.Updated);
        }

        private int getShapeIndex<T>(T entity)
        {
            var entityEntry = _entries.Where(e => e.Value is EntityEntry<T> entry && ReferenceEquals(entry.Entity, entity)).ToList();
            if (entityEntry.Count == 0)
            {
                throw new Exception();
            }
            return entityEntry[0].Key;
        }

        internal void ClearChanges()
        {
            _addEntries = new List<EntityEntry>();
            _entries = new Dictionary<int, EntityEntry>();
        }
    }
}
