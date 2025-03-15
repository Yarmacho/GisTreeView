using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    [Serializable]
    public class GasDto
    {
        [JsonPropertyName("id")]
        [XmlAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyName("x")]
        [XmlAttribute("x")]
        public double X { get; set; }

        [JsonPropertyName("Y")]
        [XmlAttribute("y")]
        public double Y { get; set; }

        [JsonPropertyName("sceneId")]
        [XmlAttribute("sceneId")]
        public int SceneId { get; set; }
    }
}
