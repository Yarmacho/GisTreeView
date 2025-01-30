using System.Text.Json.Serialization;

namespace Entities.Dtos
{
    public class GeoArea
    {
        [JsonPropertyName("minX")]
        public double MinX { get; set; }

        [JsonPropertyName("maxX")]
        public double MaxX { get; set; }

        [JsonPropertyName("minY")]
        public double MinY { get; set; }

        [JsonPropertyName("maxY")]
        public double MaxY { get; set; }
    }
}
