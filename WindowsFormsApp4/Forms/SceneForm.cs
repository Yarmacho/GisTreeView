using AxMapWinGIS;
using DynamicForms.Abstractions;
using Entities.Entities;
using MapWinGIS;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;

namespace Forms.Forms
{
    public partial class SceneForm : Form, IEntityFormWithMap<Scene>
    {
        public WindowsFormsApp4.Initializers.Map Map { get; }

        public Shape Shape { get; set; }

        public Shapefile Shapefile { get; }

        public Scene Entity { get; }

        public SceneForm(Scene scene, EditMode editMode)
        {
            InitializeComponent();
            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = false;

            // TODO: Use geodbcontext
            object result = null;
            string error = null;
            var shapefile = Map.SceneShapeFile;
            if (shapefile.Table.Query($"[SceneId] = {scene.Id}", ref result, ref error))
            {
                var shapeId = (result as int[] ?? Enumerable.Empty<int>())
                    .FirstOrDefault();

                Shape = shapefile.Shape[shapeId];
            }
            Entity = scene;
            Shapefile = this.CreateTempShapefile(shapefile);

            configureFormEvents();

            if (Map.LayersInfo.SceneLayerHandle != -1)
            {
                axMap1.set_ShapeLayerFillTransparency(Map.LayersInfo.SceneLayerHandle, 0.3f);
            }
        }

        public event Action<Point> OnMapMouseDown;
        public event Func<Point, Shape, bool> ValidShape;
        public event Action<Shape> AfterShapeValid;
        public event Action<double, double> OnMouseMoveOnMap;

        private Point _sceneCenter;

        protected void configureFormEvents()
        {
            object result = null;
            string error = null;
            var gasShapefile = Map.GasShapeFile;
            if (!gasShapefile.Table.Query($"[Id] = {Entity.GasId}", ref result, ref error))
            {
                return;
            }
            var gasShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
            var gasShape = gasShapefile.Shape[gasShapeId];
            _sceneCenter = gasShape.Point[0];

            angle.TextChanged += sceneParametersChanged;
            side.TextChanged += sceneParametersChanged;

            var coastShapefile = Map.CoastShapeFile;
            string coastQuery = "[OBJECTID]=1";
            Shape coast = null;
            if (coastShapefile.Table.Query(coastQuery, ref result, ref error))
            {
                int coastShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                coast = coastShapefile.Shape[coastShapeId];
            }

            ValidShape += (point, shape) =>
            {
                //if (coast != null && !coast.Intersects(shape))
                //{
                //    MessageBox.Show("Сцена побудована на материку. Побудуйте іншу сцену");
                //    return false;
                //}

                return shape.numPoints != 4;
            };

            AfterShapeValid += (shape) =>
            {
                Entity.Side = TypeTools.Convert<double>(side.Text);
                Entity.Angle = TypeTools.Convert<double>(angle.Text);
                Entity.Area = Shape.Area;
            };
        }

        private void sceneParametersChanged(object sender, EventArgs e)
        {
            var shape = Shape;
            buildScene(Map.GasShapeFile, ref shape, _sceneCenter,
                TypeTools.Convert<double>(angle.Text), TypeTools.Convert<double>(side.Text));
            
            Map.Redraw();
        }

        private static void buildScene(Shapefile shapefile, ref Shape shape, Point point, double angle, double sideLength)
        {
            if (shapefile.NumShapes != 0)
            {
                shapefile.StartEditingShapes();
                shapefile.EditClear();
                shapefile.StopEditingShapes();
            }

            if (angle == 0 && sideLength == 0)
            {
                return;
            }

            shapefile.StartAppendMode();
            shapefile.StartEditingShapes();
            shape = new Shape();
            shape.Create(shapefile.ShapefileType);

            var pointA = new Point();
            pointA.x = point.x + sideLength;
            pointA.y = point.y + sideLength;

            var pointB = new Point();
            pointB.x = point.x - sideLength;
            pointB.y = point.y + sideLength;

            var pointC = new Point();
            pointC.x = point.x - sideLength;
            pointC.y = point.y - sideLength;

            var pointD = new Point();
            pointD.x = point.x + sideLength;
            pointD.y = point.y - sideLength;

            var pointIndex = -1;
            shape.InsertPoint(pointA, ref pointIndex);
            shape.InsertPoint(pointB, ref pointIndex);
            shape.InsertPoint(pointC, ref pointIndex);
            shape.InsertPoint(pointD, ref pointIndex);

            shape.Rotate(point.x, point.y, angle);

            var shapeIndex = 0;
            shapefile.EditInsertShape(shape, ref shapeIndex);
            shapefile.StopEditingShapes();
            shapefile.StopAppendMode();
        }

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
            throw new NotImplementedException();
        }
    }
}
