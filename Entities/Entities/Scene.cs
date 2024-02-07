using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Scene : EntityBase<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int GasId { get; set; }
    }
}
