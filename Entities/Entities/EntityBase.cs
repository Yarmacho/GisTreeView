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
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return obj is EntityBase<TId> entityBase && entityBase.Id.Equals(Id);
        }
    }
}
