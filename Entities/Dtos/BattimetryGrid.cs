using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class BathymetryGrid
    {
        [JsonPropertyName("area")]
        public GeoArea Area { get; set; }
        [JsonPropertyName("points")]
        public List<BathymetryPoint> Points { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}
