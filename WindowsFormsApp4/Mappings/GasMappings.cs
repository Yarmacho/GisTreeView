using Entities.Entities;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            builder.Property(r => r.X).Ignore();
            builder.Property(r => r.Y).Ignore();
        }
    }
}
