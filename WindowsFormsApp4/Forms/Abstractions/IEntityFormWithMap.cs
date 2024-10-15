using MapWinGIS;
using System;

namespace DynamicForms.Abstractions
{
    public interface IEntityFormWithMap<T> : IEntityForm<T>
    {
        WindowsFormsApp4.Initializers.Map Map { get; }
        Shape Shape { get; set; }
        Shapefile Shapefile { get; }

        event Action<Point> OnMapMouseDown;
        event Func<Point, Shape, bool> ValidShape;
        event Action<Shape> AfterShapeValid;
        event Action<double, double> OnMouseMoveOnMap;

        void CallMouseDownEvents(Point point);
        bool CallValidateShapeEvents(Point point);
        void CallAfterValidShapeEvents();
        void CallOnMouseMoveEvents(double x, double y);

        void InsertPoint(Point point);
    }
}
