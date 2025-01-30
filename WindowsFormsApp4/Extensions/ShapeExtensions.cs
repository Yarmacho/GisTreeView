using MapWinGIS;
using System.Collections.Generic;
using System.Linq;

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

        public static List<Point> GetPoints(this Shape shape)
        {
            return EnumeratePoints(shape).ToList();
        }

        public static IEnumerable<Point> EnumeratePoints(this Shape shape)
        {
            var pointIndex = 0;
            while (pointIndex < shape.numPoints)
            {
                yield return shape.Point[pointIndex++];
            }
        }
    }
}
