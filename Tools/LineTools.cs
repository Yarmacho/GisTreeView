using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class LineTools
    {
        public static IEnumerable<Point> EnumeratePointsInLine(Point point1, Point point2, double step = 0.01d)
        {
            Predicate<double> condition = t => t < point2.x;
            if (point1.x > point2.x)
            {
                step *= -1;
                condition = t => t > point2.x;
            }

            yield return point1;

            var x = point1.x + step;
            for (; condition(x); x += step)
            {
                var y = ((x - point1.x) * (point2.y - point1.y) / (point2.x - point1.x)) + point1.y;

                var point = new Point();
                point.Set(x, y);

                yield return point;
            }

            yield return point2;
        }

        public static IEnumerable<Point> EnumerateRectanglePoints(Point point1, Point point2, Point point3,
            Point point4, double step = 0.01d)
        {
            var line1 = EnumeratePointsInLine(point1, point2, step).GetEnumerator();
            var line2 = EnumeratePointsInLine(point2, point3, step).GetEnumerator();

            while (line1.MoveNext() && line2.MoveNext())
            {
                foreach (var point in EnumeratePointsInLine(line1.Current, line2.Current, step))
                {
                    yield return point;
                }
            }
        }
    }
}
