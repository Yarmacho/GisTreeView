using MapWinGIS;
using System;

namespace WindowsFormsApp4.ShapeFactories
{
    internal abstract class ShapeFactoryBase : IShapesFactory
    {
        protected Shape Shape;

        public virtual bool AddPoint(double x, double y, double z = 0)
        {
            if (Shape is null)
            {
                throw new ArgumentNullException(nameof(Shape), $"Use {nameof(BeginCraete)} method before!");
            }

            var point = new Point();
            point.Set(x, y, z);

            var pointId = 0;
            return Shape.InsertPoint(point, ref pointId);
        }

        public abstract void BeginCraete();

        public virtual Shape EndCreate()
        {
            return Shape;
        }
    }
}
