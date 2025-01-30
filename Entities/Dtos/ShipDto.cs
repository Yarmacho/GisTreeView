using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    [Serializable]
    public class ShipDto
    {
        [JsonPropertyName("id")]
        [XmlAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyName("sceneId")]
        [XmlAttribute("sceneId")]
        public int SceneId { get; set; }

        [JsonPropertyName("x")]
        [XmlAttribute("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        [XmlAttribute("y")]
        public double Y { get; set; }

        [JsonPropertyName("length")]
        [XmlAttribute("length")]
        public double Lenght { get; set; }

        [JsonPropertyName("width")]
        [XmlAttribute("width")]
        public double Width { get; set; }

        [JsonPropertyName("maxSpeed")]
        [XmlAttribute("maxSpeed")]
        public double MaxSpeed { get; set; }        // максимальна швидкість в м/с

        [JsonPropertyName("turnRate")]
        [XmlAttribute("turnRate")]
        public double TurnRate { get; set; }        // максимальна швидкість повороту в радіанах/с

        [JsonPropertyName("acceleration")]
        [XmlAttribute("acceleration")]
        public double Acceleration { get; set; }    // прискорення в м/с²

        [JsonPropertyName("deceleration")]
        [XmlAttribute("deceleration")]
        public double Deceleration { get; set; }    // уповільнення в м/с²

        [JsonPropertyName("routes")]
        [XmlArray("routes")]
        [XmlArrayItem("route")]
        public List<RouteDto> Routes { get; set; }
    }
}
