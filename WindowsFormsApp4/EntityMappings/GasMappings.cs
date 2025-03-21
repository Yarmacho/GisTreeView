﻿using Entities.Entities;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Builder;

namespace WindowsFormsApp4.Mappings
{
    internal class GasMappings : TypeMapping<Gas>
    {
        protected override void PreConfigure(IMappingBuilder<Gas> builder)
        {
            builder.ToShapeFile("Gas.shp");

            builder.Property(r => r.Id).HasColumnName("Id");
            builder.Property(r => r.Name).HasColumnName("Ent_num");
            builder.Property(r => r.ExperimentId).HasColumnName("Experiment");
            builder.Property(r => r.SceneId).HasColumnName("SceneId");
            builder.Property(r => r.X).Ignore();
            builder.Property(r => r.Y).Ignore();
            builder.Property(x => x.Shape).Ignore();
        }
    }
}
