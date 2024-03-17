using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Set;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace GeoDatabase.ORM
{
    public class GeoDbContext
    {
        private readonly string _shapeFilesDirectory;
        internal readonly IServiceProvider ServiceProvider;
        private readonly ILogger<GeoDbContext> _logger;

        public GeoDbContext(string shapeFilesDirectory, IServiceProvider serviceProvider)
        {
            _shapeFilesDirectory = shapeFilesDirectory;
            ServiceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<GeoDbContext>>();
        }

        public IShapesQueryable<T> Set<T>() where T : new()
        {
            return new ShapesSet<T>(this);
        }

        public bool EnsureShapefilesStructure()
        {
            if (!Directory.Exists(_shapeFilesDirectory))
            {
                _logger.LogError("Shapes directory not found (\"{ShapesDirectory}\")", _shapeFilesDirectory);
                return false;
            }

            var mappings = ServiceProvider.GetRequiredService<MappingConfigs>();
            foreach (var mapping in mappings)
            {
                if (mapping.Shapefile == null)
                {
                    _logger.LogError("Failed to load \"{ShapesfileName}\"", mapping.ShapefileName);
                    return false;
                }

                var propertiesToCreate = mapping.ColumnIndexes
                    .Where(c => c.Value == -1)
                    .Select(c => c.Key)
                    .ToHashSet();

                if (propertiesToCreate.Count > 0)
                {
                    mapping.Shapefile.StartEditingTable();

                    var entityType = mapping.GetType().GetGenericArguments()[0];
                    foreach (var propertyName in propertiesToCreate)
                    {
                        var property = entityType.GetProperty(propertyName);
                        if (property == null)
                        {
                            throw new Exception("Invalid mapping property");
                        }

                        if (mapping.ColumnNames.TryGetValue(propertyName, out var columnName))
                        {
                            throw new Exception("Invalid property");
                        }

                        if (!mapping.ColumnPrecisions.TryGetValue(propertyName, out var precision))
                        {
                            precision = getDefaultFieldPrecision(property.PropertyType);
                        }

                        if (!mapping.ColumnLengths.TryGetValue(propertyName, out var length))
                        {
                            length = getDefaultFieldLength(property.PropertyType);
                        }

                        var fieldType = getFieldType(property.PropertyType);

                        mapping.Shapefile.EditAddField(columnName, fieldType, precision, length);
                    }

                    mapping.Shapefile.StopEditingTable();
                }
            }

            return true;
        }

        private int getDefaultFieldLength(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? 16
                : type == typeof(int)
                    ? 10
                    : type == typeof(string)
                        ? 200
                        : 1;
        }

        private int getDefaultFieldPrecision(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? 4
                : 0;
        }

        private FieldType getFieldType(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? FieldType.DOUBLE_FIELD
                : type == typeof(int)
                    ? FieldType.INTEGER_FIELD
                    : type == typeof(string)
                        ? FieldType.STRING_FIELD
                        : type == typeof(bool)
                            ? FieldType.BOOLEAN_FIELD
                            : throw new ArgumentException("Invalid property type");
        }
    }
}
