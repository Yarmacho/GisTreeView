using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using WindowsFormsApp4.JsonConverters;

namespace Entities.Dtos
{
    [Serializable]
    public class SceneDto
    {
        [JsonPropertyName("id")]
        [XmlAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyName("experimentId")]
        [XmlAttribute("experimentId")]
        public int ExperimentId { get; set; }

        [JsonPropertyName("angle")]
        [XmlAttribute("angle")]
        public double Angle { get; set; }

        [JsonPropertyName("area")]
        [XmlAttribute("area")]
        public double Area { get; set; }

        [JsonPropertyName("side")]
        [XmlAttribute("side")]
        public double Side { get; set; }

        [JsonPropertyName("ships")]
        [XmlArray("ships")]
        [XmlArrayItem("ship")]
        public List<ShipDto> Ships { get; set; }

        [JsonPropertyName("sensors")]
        [XmlArray("sensors")]
        [XmlArrayItem("sensors")]
        public List<GasDto> Sensors { get; set; }

        [JsonPropertyName("profiles")]
        [XmlArray("profiles")]
        [XmlArrayItem("profile")]
        public List<ProfilDto> Profiles { get; set; }

        [JsonPropertyName("bathymetry")]
        [JsonConverter(typeof(BathymetryStreamConverter))]
        [XmlIgnore]
        public Stream Bathymetry { get; set; }
    }
}
