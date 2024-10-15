using DynamicForms.Abstractions;
using DynamicForms.Forms;
using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4;
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
            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;

            DepthLabel = depth;
            Map = MapInitializer.Init(axMap1);
            Map.CursorMode = tkCursorMode.cmPan;
            Map.SendMouseMove = true;

            this.ConfigureMouseDownEvent();
            this.TryAddDepthIndication();
            this.ConfigureMouseMoveEvent();
            this.ConfigureSaveShapeOnFormClosed<Gas, int>();

            var shapefile = Map.GasShapeFile;
            if (gas.Id != 0)
            {
                // TODO: Find another way of resolving dbContext
                var context = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
                var entity = context.Set<Gas>().FirstOrDefault(g => g.Id == gas.Id) ?? gas;
                var shapeIndex = context.ChangeTracker.GetShapeIndex(entity);
                if (shapeIndex != -1)
                {
                    Shape = shapefile.Shape[shapeIndex];
                }
            }
            else
            {
                Entity = gas;
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
                coordX.Text = Entity.X.ToString() ?? string.Empty;
                coordY.Text = Entity.Y.ToString() ?? string.Empty;
            }

            AfterShapeValid += (s) =>
            {
                if (s.numPoints == 1)
                {
                    var point = s.Point[0];
                    Entity.X = point.x;
                    Entity.Y = point.y;
                    coordX.Text = point.x.ToString();
                    coordY.Text = point.y.ToString();
                }
            };
        }

        public System.Windows.Forms.Label DepthLabel { get; }

        public WindowsFormsApp4.Initializers.Map Map { get; }

        public Shape Shape
        {
            get => Entity.Shape;
            set => Entity.Shape = value;
        }

        public Shapefile Shapefile { get; }

        public Gas Entity { get; }

        public event Action<Point> OnMapMouseDown;
        public event Func<Point, Shape, bool> ValidShape;
        public event Action<Shape> AfterShapeValid;
        public event Action<double, double> OnMouseMoveOnMap;
        public event Action OnEntityFormClosed;

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

        private async void selectFromDict_Click(object sender, EventArgs e)
        {
            // TODO: Try to find another way of resovling service

            var gasRepository = Program.ServiceProvider
                .GetRequiredService<IGasRepository>();

            var gases = await gasRepository.GetAllAsync();

            var form = new DictionaryForm<Gas>(gases);
            try
            {
                this.Hide();
                var result = form.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                var selectedGas = form.GetSelectedRecord();
                if (selectedGas == null)
                {
                    return;
                }

                Entity.Id = selectedGas.Id;
                Entity.Name = selectedGas.Name;
                Entity.X = selectedGas.X;
                Entity.Y = selectedGas.Y;

                // TODO: Find another way of resolving dbContext
                var context = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
                context.Set<Gas>().Add(Entity);

                context.SaveChanges();
            }
            finally
            {
                this.Show();
            }
        }

        private void submit_Click(object sender, EventArgs e)
        {
            Entity.Name = name.Text;
            Entity.X = TypeTools.Convert<double>(coordX.Text);
            Entity.Y = TypeTools.Convert<double>(coordY.Text);
        }
    }
}
