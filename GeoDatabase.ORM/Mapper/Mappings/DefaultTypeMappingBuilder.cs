using GeoDatabase.ORM.Mapper.Mappings.Builder;

namespace GeoDatabase.ORM.Mapper.Mappings
{
    internal class DefaultTypeMappingBuilder<T> : TypeMapping<T>
    {
        protected override void PreConfigure(IMappingBuilder<T> builder)
        {
        }
    }
}
