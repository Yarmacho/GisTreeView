using Entities.Entities;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Builder;

namespace WindowsFormsApp4.Mappings
{
    internal class ShipMappings : TypeMapping<Ship>
    {
        protected override void PreConfigure(IMappingBuilder<Ship> builder)
        {
            builder.ToShapeFile("Ship");

            builder.Property(x => x.Id).HasColumnName("ShipId");
            builder.Property(x => x.Name).HasColumnName("Name_sh");
            builder.Property(x => x.SceneId).HasColumnName("SceneId");
            builder.Property(x => x.Acceleration).HasColumnName("Acceler");
            builder.Property(x => x.Deceleration).HasColumnName("Deceler");
            builder.Property(x => x.TurnRate).HasColumnName("TurnRate");
            builder.Property(x => x.X).Ignore();
            builder.Property(x => x.Y).Ignore();
            builder.Property(x => x.Shape).Ignore();
        }
    }
}
