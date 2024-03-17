namespace GeoDatabase.ORM.Mapper.Mappings.Builder
{
    public interface IMappingPropertyBuilder<T, TProperty>
    {
        IMappingPropertyBuilder<T, TProperty> HasColumnName(string name);
        IMappingPropertyBuilder<T, TProperty> HasPrecision(int precision);
        IMappingPropertyBuilder<T, TProperty> HasLength(int length);
        IMappingPropertyBuilder<T, TProperty> Ignore();
    }
}
