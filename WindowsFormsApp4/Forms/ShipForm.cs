using DynamicForms;
using DynamicForms.Abstractions;
using DynamicForms.Forms;
using Entities.Entities;
using GeoDatabase.ORM;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.Forms
{
    public partial class ShipForm : Form, IEntityFormWithMap<Ship>
    {
        public ShipForm(Ship ship, EditMode editMode)
        {
            InitializeComponent();
            Map = MapInitializer.Init(axMap1);
            Map.SendMouseMove = true;
            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;

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

            name.TextChanged += (s, e) => Entity.Name = name.Text;

            AfterShapeValid += (shape) =>
            {
                if (shape.numPoints == 1)
                {
                    var point = shape.Point[0];
                    Entity.X = point.x;
                    Entity.Y = point.y;
                    coordX.Text = point.x.ToString();
                    coordY.Text = point.y.ToString();

                    MapDesigner.ConnectShipWithGas(Map, Entity);
                }
            };

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

            var context = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
            context.Set<Ship>().Add(Entity);

            context.SaveChanges();
        }
    }
}
