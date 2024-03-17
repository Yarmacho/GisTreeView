using MapWinGIS;
using System;
using System.IO;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Mapper.Mappings.Builder
{
    internal class MappingBuilder<T> : IMappingBuilder<T>
    {
        private readonly MappingConfig<T> _config;
        private readonly string _shapeFilesDirectory;
        public MappingBuilder(MappingConfig<T> config, string shapeFilesDirectory)
        {
            _config = config;
            _shapeFilesDirectory = shapeFilesDirectory;
        }

        public IMappingPropertyBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
            }

            var builder = new MappingPropertyBuilder<T, TProperty>(_config, memberExpression.Member.Name);
            return builder;
        }

        public IMappingBuilder<T> ToShapeFile(string path)
        {
            var fileName = path.EndsWith(".shp") ? path : $"{path}.shp";
            var filePath = Path.Combine(_shapeFilesDirectory, path);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var shapeFile = new Shapefile();
            if (!shapeFile.Open(filePath))
            {
                throw new Exception("Failed to find shapefile");
            }

            return ToShapeFile(shapeFile);
        }

        public IMappingBuilder<T> ToShapeFile(Shapefile shapefile)
        {
            if (shapefile is null)
            {
                throw new ArgumentNullException();
            }

            _config.Shapefile = shapefile;
            _config.ShapefileName = Path.GetFileNameWithoutExtension(shapefile.Filename);
            return this;
        }
    }
}
