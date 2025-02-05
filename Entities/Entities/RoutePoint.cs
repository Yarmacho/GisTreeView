using MapWinGIS;
using System;
using System.Text.Json.Serialization;

namespace Entities.Entities
{
    [Serializable]
    public class RoutePoint : EntityBase<int>
    {
        public RoutePoint()
        {
        }

        public RoutePoint(Point point)
        {
            X = point.x;
            Y = point.y;
        }

        [JsonPropertyName("routeId")]
        public int RouteId { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        public double Speed { get; set; }
        public double Heading { get; set; }
        public double Depth { get; set; }
        public double Temperature { get; set; }
        public double Salinity { get; set; }
    }
}
