﻿using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Set;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GeoDatabase.ORM
{
    public class GeoDbContext
    {
        private readonly string _shapeFilesDirectory;
        internal readonly IServiceProvider ServiceProvider;
        private readonly ILogger<GeoDbContext> _logger;
        public readonly ChangeTracker ChangeTracker;
        private readonly Database.Database _database;

        public GeoDbContext(string shapeFilesDirectory, IServiceProvider serviceProvider)
        {
            _shapeFilesDirectory = shapeFilesDirectory;
            ServiceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<GeoDbContext>>();
            ChangeTracker = serviceProvider.GetRequiredService<ChangeTracker>();
            _database = ServiceProvider.GetRequiredService<Database.Database>();
        }

        public IShapesSet<T> Set<T>() where T : new()
        {
            return new ShapesSet<T>(this, ChangeTracker);
        }

        public bool EnsureShapefilesStructure()
        {
            return _database.EnsureShapefilesStructure();
        }

        public void DeleteAllShapes()
        {
            foreach (var mapping in ServiceProvider.GetRequiredService<MappingConfigs>())
            {
                mapping.Shapefile.StartEditingShapes();

                mapping.Shapefile.EditClear();

                mapping.Shapefile.StopEditingShapes();

                mapping.Shapefile.StartEditingTable();

                var numRows = mapping.Shapefile.Table.NumRows;
                for (var row = 0; row < numRows; row++)
                {
                    mapping.Shapefile.Table.EditDeleteRow(0);
                }

                mapping.Shapefile.StopEditingTable();
            }
        }

        public bool SaveChanges()
        {
            if (!ChangeTracker.HasChanges)
            {
                _logger.LogInformation("No changes");
                return true;
            }

            foreach (var entry in ChangeTracker.GetAllEntries().Where(e => e.State != EntityState.Attached))
            {
                _database.LoadEntity(entry);
            }

            ChangeTracker.ClearChanges();
            return true;
        }
    }
}
