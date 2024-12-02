using MapWinGIS;
using System;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Ship : EntityBase<int>, IDictionaryEntity, IEntityWithCoordinates, IShapeEntity
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int SceneId { get; set; }

        [Display(Enabled = false)]
        public double X { get; set; }

        [Display(Enabled = false)]
        public double Y { get; set; }
        public Shape Shape { get; set; }

        public double Lenght { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}SceneId: {2}{1}", Name, Environment.NewLine,
                SceneId, Id);
        }
    }
}
