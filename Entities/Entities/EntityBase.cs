using Tools;
using Tools.Attributes;

namespace Entities
{
    public abstract class EntityBase { }
    public abstract class EntityBase<TId> : EntityBase
    {
        [IgnoreProperty(EditMode.Add)]
        public TId Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EntityBase<TId> other && other.Id.Equals(Id);
        }
    }
}
