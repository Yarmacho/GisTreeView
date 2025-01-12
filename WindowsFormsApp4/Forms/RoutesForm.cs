using DynamicForms.Abstractions;
using Entities.Entities;
using GeoDatabase.ORM;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.Logic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Point = MapWinGIS.Point;

namespace WindowsFormsApp4.Forms
{
    public partial class RoutesForm : Form, IEntityFormWithMap<Route>
    {
        private readonly Ship _ship;
        private readonly Image _battimetry;

        public RoutesForm(Route route, EditMode editMode)
        {
            InitializeComponent();
            Entity = route;

            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;

            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = true;
            Map.CursorMode = tkCursorMode.cmAddShape;
            Map.AxMap.ShowZoomBar = false;

            Shapefile = this.CreateTempShapefile(Map.RoutesShapeFile);
            this.ConfigureMouseDownEvent();
            this.ConfigureSaveShapeOnFormClosed<Route, int>();

            var context = Program.ServiceProvider
                .GetRequiredService<GeoDbContext>();

            _ship = context.Set<Ship>().FirstOrDefault(s => s.Id == route.ShipId);
            if (_ship == null)
            {
                return;
            }

            var scene = context.Set<Scene>().FirstOrDefault(s => s.Id == _ship.SceneId);
            if (scene == null)
            {
                return;
            }

            var shipShape = Map.ShipShapeFile.Shape[
                context.ChangeTracker.GetShapeIndex(_ship)];

            this.СreateShape();
            InsertPoint(shipShape.Point[0]);
             
            var sceneShapeIndex = context.ChangeTracker.GetShapeIndex(scene);
            var sceneShape = Map.SceneShapeFile.Shape[sceneShapeIndex];

            _battimetry = Map.AxMap.get_Image(Map.SceneBattimetries[scene.Id]);

            Map.ZoomToShape<Scene>(sceneShapeIndex);
            ValidShape += (point, _) =>
            {
                var shape = new Shape();
                if (!shape.Create(ShpfileType.SHP_POINT))
                {
                    return false;
                }
                var pointIndex = 0;
                shape.InsertPoint(point, ref pointIndex);

                return shape.Intersects(sceneShape);
            };

            AfterShapeValid += (shape) =>
            {
                //if (shape.numPoints > 1) 
                //{
                //    var routeBuilder = new RouteBuilder(new ShipParameters()
                //    {
                //        Length = _ship.Lenght,
                //        TurnRate = 0.3,
                //        MaxSpeed = 10,
                //        Acceleration = 0.3,
                //        Deceleration = 0.1
                //    }, _battimetry);

                //    var preLastPoint = shape.Point[shape.numPoints - 2];
                //    var lastPoint = shape.Point[shape.numPoints - 1];
                //    var routePoints = routeBuilder.CalculateRoutePoints(preLastPoint, lastPoint);

                //    //shape.DeletePoint(shape.numPoints - 1);
                //    foreach (var point in routePoints.Skip(1))
                //    {
                //        var pointIndex = 0;
                //        shape.InsertPoint(new Point()
                //        {
                //            x = point.X,
                //            y = point.Y,
                //            Z = point.Depth
                //        }, ref pointIndex);
                //    }
                //}

                route.Points.Add(new Entities.Entities.RoutePoint()
                {
                    RouteId = route.Id,
                    Id = route.Points.Count,
                    X = shape.Point[shape.numPoints - 1].x,
                    Y = shape.Point[shape.numPoints - 1].y
                });
            };

            routePoints.ContextMenu = new ContextMenu();

            var item = new MenuItem("Delete");
            item.Click += deleteNode;
            routePoints.ContextMenu.MenuItems.Add(item);
        }

        public Route Entity { get; }

        public Shape Shape
        {
            get => Entity.Shape;
            set => Entity.Shape = value;
        }

        public Initializers.Map Map { get; }

        public Shapefile Shapefile { get; }

        public event Action<Point> OnMapMouseDown;
        public event Func<Point, Shape, bool> ValidShape;
        public event Action<Shape> AfterShapeValid;
        public event Action<double, double> OnMouseMoveOnMap;
        public event Action OnEntityFormClosed;

        public void CallOnFormClosedEvents()
        {
            OnEntityFormClosed.CallAllSubsribers();
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
            var pointIndex = 0;
            if (Shape.numPoints < 1)
            {
                Shape.InsertPoint(point, ref pointIndex);
                addToTreeView();
            }
            else
            {
                var routeBuilder = new RouteBuilder(new ShipParameters()
                {
                    Length = _ship.Lenght,
                    TurnRate = 0.3,
                    MaxSpeed = 10,
                    Acceleration = 0.3,
                    Deceleration = 0.1
                }, _battimetry);

                var lastPoint = Shape.Point[Shape.numPoints - 1];
                var routePoints = routeBuilder.CalculateRoutePoints(lastPoint, point);

                foreach (var routePoint in routePoints.Skip(1))
                {
                    pointIndex = Shape.AddPoint(routePoint.X, routePoint.Y);
                    addToTreeView();
                }
            }

            void addToTreeView()
            {
                routePoints.Nodes.Add(new RoutePointTreeNode(Shape, pointIndex));
            }
        }

        private void deleteNode(object sender, EventArgs e)
        {
            if (routePoints.SelectedNode != null && routePoints.SelectedNode is RoutePointTreeNode routePointTreeNode)
            {
                if (routePointTreeNode.PointIndex == 0)
                {
                    MessageBox.Show("Start point can not be deleted");
                    return;
                }

                var result = MessageBox.Show(
                    "Ви впевнені, що хочете видалити цей вузол?",
                    "Підтвердження видалення",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    routePointTreeNode.Remove();
                    Map.Redraw();
                }
            }
        }

        private class RoutePointTreeNode : TreeNode
        {
            private readonly Shape _route;
            public int PointIndex { get; private set; }

            public RoutePointTreeNode(Shape route, int pointIndex) : base(pointIndex.ToString())
            {
                _route = route;
                PointIndex = pointIndex;
            }

            public new void Remove()
            {
                if (_route.DeletePoint(PointIndex))
                {
                    if (PointIndex > 0)
                    {
                        foreach (var node in TreeView.Nodes.OfType<RoutePointTreeNode>().Where(n => n.PointIndex > PointIndex))
                        {
                            node.PointIndex--;
                            node.Text = node.PointIndex.ToString();
                        }
                    }

                    base.Remove();
                }
            }
        }
    }
}
