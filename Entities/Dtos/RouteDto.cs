using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    [Serializable]
    public class RouteDto
    {
        [JsonPropertyName("name")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyName("desc")]
        [XmlAttribute("description")]
        public string Description { get; set; }

        [JsonPropertyName("shipId")]
        [XmlAttribute("shipId")]
        public int ShipId { get; set; }

        [JsonPropertyName("points")]
        [XmlArray("points")]
        [XmlArrayItem("point")]
        public List<RoutePointDto> Points { get; set; }
    }
}
