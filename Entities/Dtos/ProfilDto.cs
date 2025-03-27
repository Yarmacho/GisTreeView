using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Entities.Dtos
{
    public class ProfilDto
    {
        public int SceneId { get; set; }
        public double Depth { get; set; }
        public double Temperature { get; set; }
        public double SoundSpeed { get; set; }
        public double Salinity { get; set; }
        
        [JsonIgnore]
        [XmlIgnore]
        public double Absorbsion { get; set; }
    }
}
