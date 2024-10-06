using DynamicForms.Abstractions;
using Entities.Entities;
using MapWinGIS;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;
using Point = MapWinGIS.Point;

namespace WindowsFormsApp4.Forms
{
    public partial class RoutesForm : Form, IEntityFormWithMap<Route>
    {
        public RoutesForm(Route route, EditMode editMode)
        {
            InitializeComponent();
            Entity = route;

            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = true;

            Shapefile = this.CreateTempShapefile(Map.RoutesShapeFile);
            this.ConfigureMouseDownEvent();

            object result = null;
            string error = null;

            var shipShapeFile = Map.ShipShapeFile;
            if (!shipShapeFile.Table.Query($"[ShipId] = {route.ShipId}", ref result, ref error))
            {
                return;
            }

            int shipShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
            if (shipShapeId == -1)
            {
                return;
            }

            var sceneIdFieldIndex = shipShapeFile.FieldIndexByName["SceneId"];
            var sceneId = TypeTools.Convert<int>(shipShapeFile.CellValue[sceneIdFieldIndex, shipShapeId]);

            var sceneShapefile = Map.SceneShapeFile;
            if (!sceneShapefile.Table.Query($"[SceneId] = {sceneId}", result, error))
            {
                return;
            }

            var sceneShapeId1 = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
            var sceneShape1 = sceneShapefile.Shape[sceneShapeId1];
            if (sceneShape1 == null)
            {
                return;
            }

            Map.AxMap.ZoomToShape(Map.LayersInfo.SceneLayerHandle, sceneShapeId1);
            ValidShape += (point, _) =>
            {
                var shape = new Shape();
                if (!shape.Create(ShpfileType.SHP_POINT))
                {
                    return false;
                }
                var pointIndex = 0;
                shape.InsertPoint(point, ref pointIndex);

                return shape.Intersects(sceneShape1);
            };

            AfterShapeValid += (shape) =>
            {
                route.Points.Add(new RoutePoint()
                {
                    RouteId = route.Id,
                    Id = route.Points.Count,
                    X = shape.Point[0].x,
                    Y = shape.Point[0].y
                });
            };

            if (Map.LayersInfo.SceneLayerHandle != -1)
            {
                axMap1.set_ShapeLayerFillTransparency(Map.LayersInfo.SceneLayerHandle, 0.3f);
            }
        }

        public Route Entity { get; }

        public Shape Shape { get; set; }

        public Initializers.Map Map { get; }

        public Shapefile Shapefile { get; }

        public event Action<Point> OnMapMouseDown;
        public event Func<Point, Shape, bool> ValidShape;
        public event Action<Shape> AfterShapeValid;
        public event Action<double, double> OnMouseMoveOnMap;

        public void CallAfterValidShapeEvents()
        {
            AfterShapeValid.CallAllSubsribers(Shape);
        }

        public void CallMouseDownEvents(Point point)
        {
            OnMapMouseDown.CallAllSubsribers(point);
        }

        public void CallOnMouseMoveEvents(double x, double y)
        {
            OnMouseMoveOnMap.CallAllSubsribers(x, y);
        }

        public bool CallValidateShapeEvents(Point point)
        {
            return ValidShape.CallAllSubsribers(point, Shape);
        }

        public void InsertPoint(Point point)
        {
            var pointIndex = 0;
            Shape.InsertPoint(point, ref pointIndex);
        }
    }
}
