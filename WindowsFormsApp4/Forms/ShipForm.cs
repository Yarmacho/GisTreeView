using DynamicForms.Abstractions;
using DynamicForms.Forms;
using Entities.Entities;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
            AcceptButton.DialogResult = DialogResult.OK;

            this.ConfigureMouseDownEvent();

            Entity = ship;
            // TODO: Use geodbcontext
            object result = null;
            string error = null;
            var shapefile = Map.ShipShapeFile;
            if (shapefile.Table.Query($"[ShipId] = {ship.Id}", ref result, ref error))
            {
                var shapeId = (result as int[] ?? Enumerable.Empty<int>())
                    .FirstOrDefault();

                Shape = shapefile.Shape[shapeId];
            }
            Shapefile = this.CreateTempShapefile(shapefile);

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

            AfterShapeValid += (shape) =>
            {
                if (shape.numPoints == 1)
                {
                    var point = shape.Point[0];
                    ship.X = point.x;
                    ship.Y = point.y;
                }
            };

            Map.AxMap.ZoomToShape(Map.LayersInfo.SceneLayerHandle,
                getSceneShapeId(ship.SceneId));

            if (Map.LayersInfo.SceneLayerHandle != -1)
            {
                axMap1.set_ShapeLayerFillTransparency(Map.LayersInfo.SceneLayerHandle, 0.3f);
            }
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
            if (Shape.numPoints == 1)
            {
                Shape.Point[0] = point;
            }
            else
            {
                Shape.InsertPoint(point, ref pointIndex);
            }
        }

        private void selectFromDict_Click(object sender, EventArgs e)
        {
            // TODO: Try to find another way of resovling service

            var gasRepository = Program.ServiceProvider
                .GetRequiredService<IShipsRepository>();

            Task.Run(async () =>
            {
                var ships = await gasRepository.GetAllAsync();

                new DictionaryForm<Ship>(ships).ShowDialog();
            });
        }
    }
}
