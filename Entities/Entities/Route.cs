using MapWinGIS;
using System;
using System.Collections.Generic;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Route : EntityBase<int>, IShapeEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int ShipId { get; set; }

        [IgnoreProperty(EditMode.View | EditMode.Add | EditMode.Edit | EditMode.Delete)]
        public List<RoutePoint> Points { get; set; } = new List<RoutePoint>();

        [IgnoreProperty(EditMode.Add | EditMode.View | EditMode.Edit | EditMode.Delete)]
        public Shape Shape { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {4}{1}Name: {0}{1}Description: {2}{1}ShipId: {3}", Name, Environment.NewLine,
                Description, ShipId, Id);
        }
    }
}
