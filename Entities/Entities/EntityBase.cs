using DynamicForms.Attributes;

namespace Entities
{
    public abstract class EntityBase<TId>
    {
        [IgnoreProperty(EditMode.Add)]
        public TId Id { get; set; }
    }
}
