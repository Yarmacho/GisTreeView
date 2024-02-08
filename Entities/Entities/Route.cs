using System.Collections.Generic;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Route : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int ShipId { get; set; }

        [IgnoreProperty(EditMode.View | EditMode.Add | EditMode.Edit | EditMode.Delete)]
        public List<RoutePoint> Points { get; set; }
    }
}
