using GeoDatabase.ORM.Set;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace GeoDatabase.ORM
{
    public class GeoDbContext
    {
        private readonly string _shapeFilesDirectory;
        internal readonly IServiceProvider ServiceProvider;
        private readonly ILogger<GeoDbContext> _logger;
        private readonly ChangeTracker _changeTracker;
        private readonly Database.Database _database;

        public GeoDbContext(string shapeFilesDirectory, IServiceProvider serviceProvider)
        {
            _shapeFilesDirectory = shapeFilesDirectory;
            ServiceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<GeoDbContext>>();
            _changeTracker = serviceProvider.GetRequiredService<ChangeTracker>();
            _database = ServiceProvider.GetRequiredService<Database.Database>();
        }

        public IShapesSet<T> Set<T>() where T : new()
        {
            return new ShapesSet<T>(this, _changeTracker);
        }

        public bool EnsureShapefilesStructure()
        {
            return _database.EnsureShapefilesStructure();
        }

        public bool SaveChanges()
        {
            if (!_changeTracker.HasChanges)
            {
                _logger.LogInformation("No changes");
                return true;
            }

            foreach (var entry in _changeTracker.GetAllEntries())
            {
                _database.LoadEntity(entry.Entity, entry.EntityType, entry.ShapeIndex, entry.Shape);
            }

            _changeTracker.ClearChanges();
            return true;
        }
    }
}
