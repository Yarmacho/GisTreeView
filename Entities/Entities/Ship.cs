using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Ship : EntityBase<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int SceneId { get; set; }
    }
}
