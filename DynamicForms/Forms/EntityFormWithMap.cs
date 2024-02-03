using MapWinGIS;

namespace DynamicForms.Forms
{
    internal class EntityFormWithMap : EntityForm, IEntityFormWithMap
    {
        internal Shape Shape;

        public EntityFormWithMap(object entity) : base(entity)
        {
        }

        public Shape GetShape()
        {
            return Shape.Clone();
        }
    }
}
