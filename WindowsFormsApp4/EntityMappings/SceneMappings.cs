using Entities.Entities;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Builder;

namespace WindowsFormsApp4.Mappings
{
    internal class SceneMappings : TypeMapping<Scene>
    {
        protected override void PreConfigure(IMappingBuilder<Scene> builder)
        {
            builder.ToShapeFile("Scene");

            builder.Property(x => x.Id).HasColumnName("SceneId");
            builder.Property(x => x.Name).HasColumnName("SceneName");
            builder.Property(x => x.Side).HasColumnName("Shape_Leng");
            builder.Property(x => x.Area).HasColumnName("Shape_Area");
            builder.Property(x => x.Angle).Ignore();
            builder.Property(x => x.ExperimentId).HasColumnName("ExpId");
            builder.Property(x => x.Shape).Ignore();
        }
    }
}
