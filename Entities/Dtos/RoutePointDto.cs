using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    [Serializable]
    public class RoutePointDto
    {
        [JsonPropertyName("routeId")]
        [XmlAttribute("routeId")]
        public int RouteId { get; set; }

        [JsonPropertyName("x")]
        [XmlAttribute("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        [XmlAttribute("y")]
        public double Y { get; set; }
    }
}
