using System;
using System.Text.Json.Serialization;

namespace Entities.Dtos
{
    public class BathymetryPoint
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("depth")]
        public double Depth { get; set; }

        [JsonPropertyName("mixX")]
        public double XRangeStart { get; set; }

        [JsonPropertyName("maxX")]
        public double XRangeEnd { get; set; }

        [JsonPropertyName("mixY")]
        public double YRangeStart { get; set; }

        [JsonPropertyName("maxY")]
        public double YRangeEnd { get; set; }
    }
}
