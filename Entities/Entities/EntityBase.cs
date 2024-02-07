using Tools;
using Tools.Attributes;

namespace Entities
{
    public abstract class EntityBase { }
    public abstract class EntityBase<TId> : EntityBase
    {
        [IgnoreProperty(EditMode.Add)]
        public TId Id { get; set; }
    }
}
