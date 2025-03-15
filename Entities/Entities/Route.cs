using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    [Serializable]
    public class Route : EntityBase<int>, IShapeEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("desc")]
        public string Description { get; set; }

        [IgnoreProperty(EditMode.Add)]
        [JsonPropertyName("shipId")]
        public int ShipId { get; set; }

        [IgnoreProperty(EditMode.View | EditMode.Add | EditMode.Edit | EditMode.Delete)]
        [JsonPropertyName("points")]
        public List<RoutePoint> Points { get; set; } = new List<RoutePoint>();

        [JsonIgnore]
        public Shape Shape { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {4}{1}Name: {0}{1}Description: {2}{1}ShipId: {3}", Name, Environment.NewLine,
                Description, ShipId, Id);
        }
    }
}
