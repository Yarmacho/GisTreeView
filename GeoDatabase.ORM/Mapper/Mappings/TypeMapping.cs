using GeoDatabase.ORM.Mapper.Mappings.Builder;
using System.Reflection;

namespace GeoDatabase.ORM.Mapper.Mappings
{
    public abstract class TypeMapping { }
    public abstract class TypeMapping<T> : TypeMapping
    {
        protected abstract void PreConfigure(IMappingBuilder<T> builder);

        internal void Configure(MappingConfig<T> config, string shapeFilesDirectory)
        {
            var builder = new MappingBuilder<T>(config, shapeFilesDirectory);
            PreConfigure(builder);

            var type = typeof(T);
            if (config.Shapefile == null)
            {
                builder.ToShapeFile($"{type.Name}.shp");
            }

            var shapeFile = config.Shapefile;
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (config.IgnoredProperties.Contains(property.Name))
                {
                    continue;
                }

                if (!config.ColumnNames.TryGetValue(property.Name, out var columnName))
                {
                    columnName = property.Name;
                }

                var index = shapeFile.FieldIndexByName[columnName];
                //if (index == -1)
                //{
                //    throw new System.Exception($"Field with name \"{columnName}\" not found");
                //}

                config.ColumnIndexes.Add(property.Name, index);
            }
        }
    }
}