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

        [JsonPropertyName("speed")]
        [XmlAttribute("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("depth")]
        [XmlAttribute("depth")]
        public double Depth { get; set; }

        [JsonPropertyName("temperature")]
        [XmlAttribute("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("salinity")]
        [XmlAttribute("salinity")]
        public double Salinity { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan TimeOffset { get; set; }

        [JsonPropertyName("timeOffsetSeconds")]
        [XmlAttribute("timeOffsetSeconds")]
        public double TimeOffsetSeconds
        {
            get { return TimeOffset.TotalSeconds; }
            set { TimeOffset = TimeSpan.FromSeconds(value); }
        }
    }
}
