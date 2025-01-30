using MapWinGIS;
using System;
using System.Text.Json.Serialization;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    [Serializable]
    public class Scene : EntityBase<int>, IShapeEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("experimentId")]
        public int ExperimentId { get; set; }

        [IgnoreProperty(EditMode.Add)]
        [JsonIgnore]
        public int GasId { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("angle")]
        public double Angle { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("area")]
        public double Area { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("side")]
        public double Side { get; set; }

        [JsonIgnore]
        public Shape Shape { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}GasId: {2}{1}", Name, Environment.NewLine,
                GasId, Id);
        }
    }
}
