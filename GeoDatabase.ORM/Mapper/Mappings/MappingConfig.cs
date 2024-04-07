using MapWinGIS;
using System.Collections.Generic;

namespace GeoDatabase.ORM.Mapper.Mappings
{
    internal abstract class MappingConfig
    {
        public Shapefile Shapefile { get; internal set; }
        internal string ShapefileName;

        internal Dictionary<string, string> ColumnNames = new Dictionary<string, string>();
        internal HashSet<string> IgnoredProperties = new HashSet<string>();

        internal Dictionary<string, int> ColumnIndexes = new Dictionary<string, int>();
        internal Dictionary<string, int> ColumnPrecisions = new Dictionary<string, int>();
        internal Dictionary<string, int> ColumnLengths = new Dictionary<string, int>();
    }

    internal class MappingConfig<T> : MappingConfig, IMappingConfig<T>
    {
    }
}
