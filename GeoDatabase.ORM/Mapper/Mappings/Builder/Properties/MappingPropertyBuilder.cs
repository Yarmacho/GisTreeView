namespace GeoDatabase.ORM.Mapper.Mappings.Builder
{
    internal class MappingPropertyBuilder<T, TProperty> : IMappingPropertyBuilder<T, TProperty>
    {
        private readonly MappingConfig<T> _config;
        private readonly string _propertyName;

        public MappingPropertyBuilder(MappingConfig<T> config, string propertyName)
        {
            _config = config;
            _propertyName = propertyName;
        }

        public IMappingPropertyBuilder<T, TProperty> HasColumnName(string name)
        {
            _config.ColumnNames[_propertyName] = name;
            return this;
        }

        public IMappingPropertyBuilder<T, TProperty> HasLength(int length)
        {
            _config.ColumnLengths[_propertyName] = length;
            return this;
        }

        public IMappingPropertyBuilder<T, TProperty> HasPrecision(int precision)
        {
            _config.ColumnPrecisions[_propertyName] = precision;
            return this;
        }

        public IMappingPropertyBuilder<T, TProperty> Ignore()
        {
            _config.IgnoredProperties.Add(_propertyName);
            return this;
        }

        public IMappingPropertyBuilder<T, TProperty> HasShapeProperty(string shapePropertyName)
        {
            _config.ShapePropertyName = shapePropertyName;
            return this;
        }
    }
}
