﻿using DynamicForms.Abstractions;
using Entities.Entities;
using GeoDatabase.ORM;
using MapWinGIS;
using MassTransit.Internals.GraphValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tools;
using Tools.Extensions;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.Logic;
using Point = MapWinGIS.Point;
using Image = MapWinGIS.Image;
using WindowsFormsApp4.Forms.Abstractions;

namespace WindowsFormsApp4.Forms
{
    public partial class RoutesForm : Form, IEntityFormWithMap<Route>, IEntityFormWithMapAndDepthLabel<Route>
    {
        private readonly Ship _ship;
        private readonly Image _battimetry;
        private readonly Shape _sceneShape;

        public RoutesForm(Route route, EditMode editMode)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedDialog;
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
            addShape.Click += (s, e) => Map.CursorMode = tkCursorMode.cmAddShape;
            panBtn.Click += (s, e) => Map.CursorMode = tkCursorMode.cmPan;

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
            _sceneShape = Map.SceneShapeFile.Shape[sceneShapeIndex];

            _battimetry = Map.AxMap.get_Image(Map.SceneBattimetries[scene.Id]);

            _routeBuilder = new RouteBuilder(new ShipParameters(_ship), _battimetry, Map,
                _sceneShape);

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

                return shape.Intersects(_sceneShape);
            };

            AfterShapeValid += (shape) =>
            {
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

            this.ConfigureMouseMoveEvent();
            this.TryAddDepthIndication(scene.Id);
        }

        public Route Entity { get; }

        public Shape Shape
        {
            get => Entity.Shape;
            set => Entity.Shape = value;
        }

        public Initializers.Map Map { get; }

        private RouteBuilder _routeBuilder;

        public Shapefile Shapefile { get; }

        public System.Windows.Forms.Label DepthLabel => depth;

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
                addToTreeView(pointIndex, true);
            }
            else
            {
                var lastPoint = Shape.Point[Shape.numPoints - 1];
                var routePoints = _routeBuilder.CalculateRouteBetweenPoints(lastPoint, point);

                for (var i = 1; i < routePoints.Count; i++)
                {
                    var routePoint = routePoints[i];

                    pointIndex = Shape.AddPoint(routePoint.X, routePoint.Y);
                    addToTreeView(pointIndex, i == (routePoints.Count - 1));
                }
            }
        }

        private void addToTreeView(int pointIndex, bool isWayPoint)
        {
            if (isWayPoint)
            {
                routePoints.Nodes.Add(new RoutePointTreeNode(Shape, pointIndex, isWayPoint));
            }
            else
            {
                routePoints.Nodes[routePoints.Nodes.Count - 1]
                    .Nodes.Add(new RoutePointTreeNode(Shape, pointIndex, isWayPoint));
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

                if (!routePointTreeNode.IsWayPoint)
                {
                    MessageBox.Show("Only way points can be deleted");
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure that you want to delete a route point?",
                    "Delete confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    var nodeIndex = routePointTreeNode.Index;

                    var nextNode = routePointTreeNode.NextNode as RoutePointTreeNode;
                    var prevNode = routePointTreeNode.PrevNode as RoutePointTreeNode;

                    routePointTreeNode.Remove();

                    // Rebuild the route shape
                    if (nextNode != null) 
                    {
                        var endPoint = Shape.Point[nextNode.PointIndex];
                        var startPoint = Shape.Point[prevNode.PointIndex];

                        _routeBuilder = new RouteBuilder(new ShipParameters(_ship), _battimetry, Map,
                            _sceneShape);

                        var shapeClone = Shape.Clone();
                        Shape.DeleteAllPoints();

                        var newNodes = new List<RoutePointTreeNode>();
                        for (var i = 0; i < routePoints.Nodes.Count; i++)
                        {
                            var node = routePoints.Nodes[i] as RoutePointTreeNode;
                            if (node.PointIndex > prevNode.PointIndex)
                            {
                                break;
                            }

                            var point = shapeClone.Point[node.PointIndex];
                            var pointIndex = Shape.AddPoint(point.x, point.y);

                            var newNode = new RoutePointTreeNode(Shape, pointIndex, true);
                            newNodes.Add(newNode);

                            foreach (var childNode in node.Nodes.OfType<RoutePointTreeNode>())
                            {
                                point = shapeClone.Point[childNode.PointIndex];
                                pointIndex = Shape.AddPoint(point.x, point.y);

                                newNode.Nodes.Add(new RoutePointTreeNode(Shape, pointIndex, false));
                            }
                        }

                        var lastNode = newNodes[newNodes.Count - 1];
                        var route = _routeBuilder.CalculateRouteBetweenPoints(startPoint, endPoint);
                        for (var i = 1; i < route.Count - 1; i++)
                        {
                            var routePoint = route[i];

                            var pointIndex = Shape.AddPoint(routePoint.X, routePoint.Y);

                            lastNode.Nodes.Add(new RoutePointTreeNode(Shape, pointIndex, false));
                        }

                        var tempNode = nextNode;
                        while (tempNode != null)
                        {
                            var point = shapeClone.Point[tempNode.PointIndex];
                            var pointIndex = Shape.AddPoint(point.x, point.y);

                            var newNode = new RoutePointTreeNode(Shape, pointIndex, true);
                            newNodes.Add(newNode);

                            foreach (var childNode in tempNode.Nodes.OfType<RoutePointTreeNode>())
                            {
                                point = shapeClone.Point[childNode.PointIndex];
                                pointIndex = Shape.AddPoint(point.x, point.y);

                                newNode.Nodes.Add(new RoutePointTreeNode(Shape, pointIndex, false));
                            }

                            tempNode = tempNode.NextNode as RoutePointTreeNode;
                        }

                        routePoints.Nodes.Clear();
                        routePoints.Nodes.AddRange(newNodes.ToArray());
                    }

                    Map.Redraw();
                }
            }
        }

        private class RoutePointTreeNode : TreeNode
        {
            private readonly Shape _route;
            public readonly bool IsWayPoint;

            public int PointIndex { get; private set; }

            public RoutePointTreeNode(Shape route, int pointIndex, bool isWayPoint) : base(pointIndex.ToString())
            {
                _route = route;
                PointIndex = pointIndex;
                IsWayPoint = isWayPoint;

                ForeColor = isWayPoint ? System.Drawing.Color.Black : System.Drawing.Color.DarkGray;
            }

            public new void Remove()
            {
                if (!IsWayPoint)
                {
                    return;
                }

                if (PointIndex > 0)
                {
                    var prevNode = PrevNode as RoutePointTreeNode;

                    deleteNodePoints(this);
                    _route.DeletePoint(PointIndex);
                    deleteNodePoints(prevNode);

                    prevNode.Nodes.Clear();
                    Nodes.Clear();
                    base.Remove();

                    var pointIndex = prevNode.PointIndex;
                    foreach (var node in prevNode.TreeView.Nodes.OfType<RoutePointTreeNode>().Where(n => n.Index > prevNode.Index))
                    {
                        node.PointIndex = ++pointIndex;
                        node.Text = node.PointIndex.ToString();
                        foreach (var childNode in node.Nodes.OfType<RoutePointTreeNode>())
                        {
                            childNode.PointIndex = ++pointIndex;
                            childNode.Text = childNode.PointIndex.ToString();
                        }
                    }
                }
            }

            private void deleteNodePoints(RoutePointTreeNode node)
            {
                foreach (var childNode in node.Nodes.OfType<RoutePointTreeNode>().Reverse())
                {
                    _route.DeletePoint(childNode.PointIndex);
                }
            }
        }
    }
}
