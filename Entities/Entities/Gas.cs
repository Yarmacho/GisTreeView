using MapWinGIS;
using System;
using System.Text.Json.Serialization;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Gas : EntityBase<int>, IDictionaryEntity, IEntityWithCoordinates, IShapeEntity
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int ExperimentId { get; set; }

        [Display(Enabled = false)]
        public double X { get; set; }

        [Display(Enabled = false)]
        public double Y { get; set; }
        [JsonPropertyName("sceneId")]
        public int SceneId { get; set; }
        public Shape Shape { get; set; }

        /// <summary>
        /// Глибина розміщення датчика (метри від поверхні води)
        /// </summary>
        public double Depth { get; set; }

        /// <summary>
        /// Кут нахилу датчика по вертикалі (градуси)
        /// </summary>
        public double VerticalAngle { get; set; }

        /// <summary>
        /// Кут повороту датчика по горизонталі (градуси)
        /// </summary>
        public double HorizontalAngle { get; set; }

        /// <summary>
        /// Мінімальна робоча частота датчика (Гц)
        /// </summary>
        public double MinFrequency { get; set; }

        /// <summary>
        /// Максимальна робоча частота датчика (Гц)
        /// </summary>
        public double MaxFrequency { get; set; }

        /// <summary>
        /// Чутливість датчика (дБ відносно 1В/мкПа)
        /// </summary>
        public double Sensitivity { get; set; }

        /// <summary>
        /// Динамічний діапазон датчика (дБ)
        /// </summary>
        public double DynamicRange { get; set; }

        /// <summary>
        /// Ширина діаграми спрямованості на рівні -3дБ (градуси)
        /// </summary>
        public double BeamWidth { get; set; }

        /// <summary>
        /// Коефіцієнт спрямованості (дБ)
        /// </summary>
        public double DirectivityIndex { get; set; }

        /// <summary>
        /// Максимальна дальність виявлення (м)
        /// </summary>
        public double MaxDetectionRange { get; set; }


        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}ExperimentId: {2}{1}", Name, Environment.NewLine,
                ExperimentId, Id);
        }
    }
}
