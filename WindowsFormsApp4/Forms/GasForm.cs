using AxMapWinGIS;
using DynamicForms.Abstractions;
using Entities.Entities;
using MapWinGIS;
using System;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Forms.Abstractions;
using WindowsFormsApp4.Initializers;

namespace Forms.Forms
{
    public partial class GasForm : Form, IEntityFormWithMapAndDepthLabel<Gas>
    {
        public GasForm(Gas gas, EditMode editMode)
        {
            InitializeComponent();
            DepthLabel = depth;
            Map = MapInitializer.Init(axMap1);
            Map.CursorMode = tkCursorMode.cmPan;
            Map.SendMouseMove = true;

            this.ConfigureMouseDownEvent();
            this.TryAddDepthIndication(Map);
            this.ConfigureMouseMoveEvent();

            // TODO: Use geodbcontext
            object result = null;
            string error = null;
            var shapefile = Map.GasShapeFile;
            if (shapefile.Table.Query($"[Id] = {gas.Id}", ref result, ref error))
            {
                var shapeId = (result as int[] ?? Enumerable.Empty<int>())
                    .FirstOrDefault();

                Shape = shapefile.Shape[shapeId];
            }
            Entity = gas;
            Shapefile = this.CreateTempShapefile(shapefile);

            if (editMode == EditMode.Add)
            {
                experimentId.Visible = false;
                coordX.Text = string.Empty;
                coordY.Text = string.Empty;
            }
            else
            {
                coordX.Text = gas.X.ToString() ?? string.Empty;
                coordY.Text = gas.Y.ToString() ?? string.Empty;
            }

            AfterShapeValid += (s) =>
            {
                if (s.numPoints == 1)
                {
                    var point = s.Point[0];
                    gas.X = point.x;
                    gas.Y = point.y;
                }
            };

            if (Map.LayersInfo.SceneLayerHandle != -1)
            {
                axMap1.set_ShapeLayerFillTransparency(Map.LayersInfo.SceneLayerHandle, 0.3f);
            }
        }

        public System.Windows.Forms.Label DepthLabel { get; }

        public WindowsFormsApp4.Initializers.Map Map { get; }

        public Shape Shape { get; set; }

        public Shapefile Shapefile { get; }

        public Gas Entity { get; }

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

        private void panBtn_Click(object sender, EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmPan;
        }

        private void addShape_Click(object sender, EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmAddShape;
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmZoomIn;
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmZoomOut;
        }
    }
}
