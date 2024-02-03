using MapWinGIS;

namespace DynamicForms.Forms
{
    public interface IEntityFormWithMap : IEntityForm
    {
        Shape GetShape();
    }
}
