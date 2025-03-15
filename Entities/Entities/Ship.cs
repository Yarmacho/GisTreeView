using MapWinGIS;
using System;
using System.Text.Json.Serialization;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    [Serializable]
    public class Ship : EntityBase<int>, IDictionaryEntity, IEntityWithCoordinates, IShapeEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        [JsonPropertyName("sceneId")]
        public int SceneId { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("x")]
        public double X { get; set; }

        [Display(Enabled = false)]
        [JsonPropertyName("y")]
        public double Y { get; set; }
        [JsonIgnore]
        public Shape Shape { get; set; }

        [JsonPropertyName("length")]
        public double Lenght { get; set; }

        [JsonPropertyName("width")]
        public double Width { get; set; }
        
        [JsonPropertyName("maxSpeed")]
        public double MaxSpeed { get; set; }        // максимальна швидкість в м/с
        
        [JsonPropertyName("turnRate")]
        public double TurnRate { get; set; }        // максимальна швидкість повороту в радіанах/с
        
        [JsonPropertyName("acceleration")]
        public double Acceleration { get; set; }    // прискорення в м/с²

        [JsonPropertyName("deceleration")]
        public double Deceleration { get; set; }    // уповільнення в м/с²

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}SceneId: {2}{1}", Name, Environment.NewLine,
                SceneId, Id);
        }
    }
}
