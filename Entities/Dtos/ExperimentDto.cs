using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    [Serializable]
    [XmlRoot("experiment")]
    public class ExperimentDto
    {
        [JsonPropertyName("id")]
        [XmlAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyName("desc")]
        [XmlAttribute("description")]
        public string Description { get; set; }

        [JsonPropertyName("scenes")]
        [XmlArray("scenes")]
        [XmlArrayItem("scene")]
        public List<SceneDto> Scenes { get; set; }
    }
}
