using MapWinGIS;
using System;
using System.Text.Json.Serialization;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    [Serializable]
    public class Gas : EntityBase<int>, IDictionaryEntity, IEntityWithCoordinates, IShapeEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        [JsonIgnore]
        public int ExperimentId { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("x")]
        public double X { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("y")]
        public double Y { get; set; }
        [JsonPropertyName("sceneId")]
        public int SceneId { get; set; }
        [JsonIgnore]
        public Shape Shape { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}ExperimentId: {2}{1}", Name, Environment.NewLine,
                ExperimentId, Id);
        }
    }
}
