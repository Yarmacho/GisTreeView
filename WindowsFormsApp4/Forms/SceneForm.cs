using AxMapWinGIS;
using DynamicForms.Abstractions;
using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;

namespace Forms.Forms
{
    public partial class SceneForm : Form, IEntityFormWithMap<Scene>
    {
        public WindowsFormsApp4.Initializers.Map Map { get; }

        public Shape Shape
        {
            get => Entity.Shape;
            set => Entity.Shape = value;
        }

        public Shapefile Shapefile { get; }

        public Scene Entity { get; }

        public SceneForm(Scene scene, EditMode editMode)
        {
            InitializeComponent();
            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = false;
            Map.CursorMode = tkCursorMode.cmPan;
            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;

            var context = Program.ServiceProvider
                .GetRequiredService<GeoDbContext>();
            
            Entity = scene;
            var shapefile = Map.SceneShapeFile;
            Shapefile = this.CreateTempShapefile(shapefile, 0.3f);

            if (scene.Id != 0)
            {
                var sceneEntity = context.Set<Scene>()
                    .FirstOrDefault(s => s.Id == scene.Id);

                var shapeIndex = context.ChangeTracker.GetShapeIndex(sceneEntity);
                if (shapeIndex != -1)
                {
                    Shape = shapefile.Shape[shapeIndex];
                }
            }

            configureFormEvents(context);

            buildScene(TypeTools.Convert<double>(angle.Text), TypeTools.Convert<double>(side.Text));
            Map.Redraw();
        }

        public event Action<Point> OnMapMouseDown;
        public event Func<Point, Shape, bool> ValidShape;
        public event Action<Shape> AfterShapeValid;
        public event Action<double, double> OnMouseMoveOnMap;
        public event Action OnEntityFormClosed;

        private Point _sceneCenter;

        protected void configureFormEvents(GeoDbContext context)
        {
            this.ConfigureSaveShapeOnFormClosed<Scene, int>();

            if (Entity == null || Entity.GasId == 0)
            {
                return;
            }

            var gas = context.Set<Gas>()
                .FirstOrDefault(g => g.Id == Entity.GasId);
            var gasShapeIndex = context.ChangeTracker.GetShapeIndex(gas);
            if (gasShapeIndex == -1)
            {
                throw new Exception("Scene is not associated to a GAS");
            }

            Map.ZoomToShape<Gas>(gasShapeIndex);

            _sceneCenter = Map.GasShapeFile.Shape[gasShapeIndex]
                .Point[0];

            name.TextChanged += (s, e) => Entity.Name = name.Text;

            angle.TextChanged += sceneParametersChanged;
            side.TextChanged += sceneParametersChanged;

            object result = null;
            string error = null;

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
            buildScene(TypeTools.Convert<double>(angle.Text), TypeTools.Convert<double>(side.Text));
            Map.Redraw();
        }

        private void buildScene(double angle, double sideLength)
        {
            if (Shapefile.NumShapes != 0)
            {
                Shapefile.StartEditingShapes();
                Shapefile.EditClear();
                Shapefile.StopEditingShapes();
            }

            if (angle == 0 && sideLength == 0)
            {
                return;
            }

            Shapefile.StartAppendMode();
            Shapefile.StartEditingShapes();
            Shape = new Shape();
            Shape.Create(Shapefile.ShapefileType);

            var point = _sceneCenter;

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
            Shape.InsertPoint(pointA, ref pointIndex);
            Shape.InsertPoint(pointB, ref pointIndex);
            Shape.InsertPoint(pointC, ref pointIndex);
            Shape.InsertPoint(pointD, ref pointIndex);

            Shape.Rotate(point.x, point.y, angle);

            var shapeIndex = 0;
            Shapefile.EditInsertShape(Shape, ref shapeIndex);
            Shapefile.StopEditingShapes();
            Shapefile.StopAppendMode();
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

        public void CallOnFormClosedEvents()
        {
            OnEntityFormClosed.CallAllSubsribers();
        }

        public void InsertPoint(Point point)
        {
            throw new NotImplementedException();
        }
    }
}
