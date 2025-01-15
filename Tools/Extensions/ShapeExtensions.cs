using MapWinGIS;

namespace Tools.Extensions
{
    public static class ShapeExtensions
    {
        public static void DeleteAllPoints(this Shape shape)
        {
            for (var i = shape.numPoints - 1; i >= 0; i--)
            {
                shape.DeletePoint(i);
            }
        }
    }
}
