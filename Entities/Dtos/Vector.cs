using System;

namespace Entities.Dtos
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector()
        {
        }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public static Vector Normalize(Vector v)
        {
            double length = v.Length;
            return new Vector(v.X / length, v.Y / length);
        }

        public Vector Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y);
            return new Vector { X = X / length, Y = Y / length };
        }

        public static double AngleBetween(Vector v1, Vector v2)
        {
            double dot = v1.X * v2.X + v1.Y * v2.Y;
            double det = v1.X * v2.Y - v1.Y * v2.X;
            return Math.Atan2(det, dot);
        }
    }
}
