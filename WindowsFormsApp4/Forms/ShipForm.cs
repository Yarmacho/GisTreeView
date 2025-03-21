﻿using DynamicForms;
using DynamicForms.Abstractions;
using DynamicForms.Forms;
using Entities.Entities;
using GeoDatabase.ORM;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Forms.Abstractions;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.Forms
{
    public partial class ShipForm : Form, IEntityFormWithMap<Ship>, IEntityFormWithMapAndDepthLabel<Ship>
    {
        public ShipForm(Ship ship, EditMode editMode)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = true;
            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;
            if (editMode == EditMode.Edit)
            {
                submit.Text = "Update";
            }

            addShape.Click += (s, e) => Map.CursorMode = tkCursorMode.cmAddShape;
            panBtn.Click += (s, e) => Map.CursorMode = tkCursorMode.cmPan;

            this.ConfigureMouseDownEvent();
            Map.CursorMode = tkCursorMode.cmAddShape;
            Map.AxMap.ShowZoomBar = false;

            Entity = ship;
            var shapefile = Map.GasShapeFile;
            if (ship.Id != 0)
            {
                // TODO: Find another way of resolving dbContext
                var context = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
                var entity = context.Set<Ship>().FirstOrDefault(g => g.Id == ship.Id);
                if (entity != null)
                {
                    var shapeIndex = context.ChangeTracker.GetShapeIndex(entity);
                    if (shapeIndex != -1)
                    {
                        Shape = shapefile.Shape[shapeIndex];
                    }
                }
            }

            Shapefile = this.CreateTempShapefile(shapefile);
            this.ConfigureSaveShapeOnFormClosed<Ship, int>();

            if (editMode == EditMode.Add)
            {
                experimentId.Visible = false;
                coordX.Text = string.Empty;
                coordY.Text = string.Empty;
            }
            else
            {
                coordX.Text = ship.X.ToString() ?? string.Empty;
                coordY.Text = ship.Y.ToString() ?? string.Empty;
            }

            ValidShape += (point, _) =>
            {
                var sceneShape = Map.SceneShapeFile.Shape[getSceneShapeId(ship.SceneId)];

                var shape = new Shape();
                if (!shape.Create(ShpfileType.SHP_POINT))
                {
                    return false;
                }
                var pointIndex = 0;
                shape.InsertPoint(point, ref pointIndex);

                return shape.Intersects(sceneShape);
            };

            name.Text = Entity.Name;
            length.Text = Entity.Lenght.ToString();
            coordX.Text = Entity.X.ToString();
            coordY.Text = Entity.Y.ToString();
            maxSpeed.Text = Entity.MaxSpeed.ToString();
            width.Text = Entity.Width.ToString();
            turnRate.Text = Entity.TurnRate.ToString();
            deceleration.Text = Entity.Deceleration.ToString();
            acceleration.Text = Entity.Acceleration.ToString();

            name.TextChanged += (s, e) => Entity.Name = name.Text;
            length.TextChanged += (s, e) => Entity.Lenght = length.Value;
            maxSpeed.TextChanged += (s, e) => Entity.MaxSpeed = maxSpeed.Value;
            turnRate.TextChanged += (s, e) => Entity.TurnRate = turnRate.Value;
            deceleration.TextChanged += (s, e) => Entity.Deceleration = deceleration.Value;
            acceleration.TextChanged += (s, e) => Entity.Acceleration = acceleration.Value;
            width.TextChanged += (s, e) => Entity.Width = width.Value;

            AfterShapeValid += (shape) =>
            {
                if (shape.numPoints == 1)
                {
                    var point = shape.Point[0];
                    Entity.X = point.x;
                    Entity.Y = point.y;
                    coordX.Text = point.x.ToString();
                    coordY.Text = point.y.ToString();

                    MapDesigner.ConnectShipWithGases(Map, Entity);
                }
            };

            this.ConfigureMouseMoveEvent();
            this.TryAddDepthIndication(Entity.SceneId);

            Map.AxMap.ZoomToShape(Map.LayersInfo.SceneLayerHandle,
                getSceneShapeId(Entity.SceneId));
        }

        private int getSceneShapeId(int sceneId)
        {
            object result = null;
            string error = null;

            var sceneShapeFile = Map.SceneShapeFile;
            if (!sceneShapeFile.Table.Query($"[SceneId] = {sceneId}", ref result, ref error))
            {
                MessageBox.Show("Scene not found");
                return -1;
            }

            return (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
        }

        public Ship Entity { get; }

        public Shape Shape
        {
            get => Entity.Shape;
            set => Entity.Shape = value;
        }

        public Initializers.Map Map { get; }

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
            if (Shape.numPoints == 1)
            {
                Shape.Point[0] = point;
            }
            else
            {
                Shape.InsertPoint(point, ref pointIndex);
            }
        }

        private async void selectFromDict_Click(object sender, EventArgs e)
        {
            // TODO: Try to find another way of resovling service

            var gasRepository = Program.ServiceProvider
                .GetRequiredService<IShipsRepository>();

            var ships = await gasRepository.GetAllAsync();

            var form = new DictionaryForm<Ship>(ships);
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var selectedShip = form.GetSelectedRecord();
            if (selectedShip == null)
            {
                return;
            }

            Entity.Id = selectedShip.Id;
            Entity.Name = selectedShip.Name;
            Entity.X = selectedShip.X;
            Entity.Y = selectedShip.Y;
            Entity.Lenght = selectedShip.Lenght;
            Entity.Width = selectedShip.Width;
            Entity.TurnRate = selectedShip.TurnRate;
            Entity.Acceleration = selectedShip.Acceleration;
            Entity.Deceleration = selectedShip.Deceleration;
            Entity.MaxSpeed = selectedShip.MaxSpeed;

            var context = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
            context.Set<Ship>().Add(Entity);

            context.SaveChanges();
        }
    }
}
