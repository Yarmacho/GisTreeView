using GeoDatabase.ORM.Mapper.Mappings;

namespace GeoDatabase.ORM.Mapper
{
    public interface IMappingConfigProvider
    {
        IMappingConfig<T> GetConfig<T>();
    }
}
