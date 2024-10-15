using Entities.Entities;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Builder;

namespace WindowsFormsApp4.Mappings
{
    internal class RouteMappings : TypeMapping<Route>
    {
        protected override void PreConfigure(IMappingBuilder<Route> builder)
        {
            builder.ToShapeFile("TraceLine");

            builder.Property(r => r.Id).HasColumnName("Id");
            builder.Property(r => r.Name).Ignore();
            builder.Property(r => r.ShipId).Ignore();
            builder.Property(r => r.Description).Ignore();
            builder.Property(r => r.Points).Ignore();
        }
    }
}
