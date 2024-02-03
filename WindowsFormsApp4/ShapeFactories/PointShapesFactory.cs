using MapWinGIS;

namespace WindowsFormsApp4.ShapeFactories
{
    internal class PointShapesFactory : ShapeFactoryBase
    {
        public override void BeginCraete()
        {
            Shape = new Shape();
            Shape.ShapeType = ShpfileType.SHP_POINT;
        }
    }
}
