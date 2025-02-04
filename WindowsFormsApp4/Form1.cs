using Entities;
using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes;
using WindowsFormsApp4.TreeNodes.Abstractions;
using Tools;
using WindowsFormsApp4.Initializers;
using Forms.Forms;
using WindowsFormsApp4.Forms.Abstractions;
using DynamicForms;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Events;
using AxMapWinGIS;
using System.Data.Common;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form, IEntityFormWithMapAndDepthLabel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _path;

        private Initializers.Map _initedMap;

        public event Action<double, double> OnMouseMoveOnMap;

        public Initializers.Map Map => _initedMap;

        public System.Windows.Forms.Label DepthLabel => depth;

        public Form1(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Text = "Experiments manager";
            _serviceProvider = serviceProvider;
            _path = configuration.GetValue<string>("MapsPath");
            InitializeComponent();
            treeView1.ServiceProvider = _serviceProvider;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            axMap1.CursorMode = tkCursorMode.cmPan;
            axMap1.SendMouseMove = true;
            _initedMap = MapInitializer.Init(axMap1);

            var temperatureMap = MapInitializer.InitBattimetries(axMap2, initTemperature: true);
            axMap2.ShowCoordinates = tkCoordinatesDisplay.cdmNone;
            axMap2.ShowCoordinatesBackground = false;
            axMap2.ShowZoomBar = false;

            var salinityMap = MapInitializer.InitBattimetries(axMap3, initSalnity: true);
            axMap3.ShowCoordinates = tkCoordinatesDisplay.cdmNone;
            axMap3.ShowCoordinatesBackground = false;
            axMap3.ShowZoomBar = false;

            var temperatureSynchronizer = new MapSynchronizer(axMap1, axMap2);
            temperatureSynchronizer.OnMouseMove += point =>
            {
                var temperature = temperatureMap.Temperature;
                if (temperature == null)
                {
                    temperatureLabel.Text = "Temperature:";
                    return;
                }

                temperature.ProjectionToImage(point.x, point.y, out var column, out var row);

                var band = temperature.ActiveBand ?? temperature.Band[1];

                var temperatureValue = 0d;
                var hasValue = band != null && band.Value[column, row, out temperatureValue];

                temperatureLabel.Text = hasValue ? $"Temperature: {temperatureValue}" : "Temperature undefined";
            };

            var salnitySynchronizer = new MapSynchronizer(axMap1, axMap3);
            salnitySynchronizer.OnMouseMove += point =>
            {
                var salinity = salinityMap.Salnity;
                if (salinity == null)
                {
                    salnityLabel.Text = "Salinity:";
                    return;
                }

                salinity.ProjectionToImage(point.x, point.y, out var column, out var row);

                var band = salinity.ActiveBand ?? salinity.Band[1];

                var salinityValue = 0d;
                var hasValue = band != null && band.Value[column, row, out salinityValue];

                salnityLabel.Text = hasValue ? $"Salinity: {salinityValue}" : "Salinity undefined";
            };

            this.TryAddDepthIndication();
            axMap1.MouseMoveEvent += (s, e1) =>
            {
                var projX = 0d;
                var projY = 0d;
                axMap1.PixelToProj(e1.x, e1.y, ref projX, ref projY);

                OnMouseMoveOnMap.CallAllSubsribers(projX, projY);
            };

            treeView1.Map = _initedMap;
            var gasShp = _initedMap.GasShapeFile;

            var nodes = await new MapObjectsTreeBuilder().BuidNodes(new BuildNodesParams()
            {
                Map = _initedMap,
                ShowExperiments = true,
                ServiceProvider = _serviceProvider
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
            treeView1.AfterSelect += TreeView1_AfterSelect;

            var layerCheckBoxTop = 0;
            foreach (var layer in _initedMap.LayersInfo)
            {
                var checkBox = new CheckBox()
                {
                    Text = layer.Key,
                    Checked = true
                };
                checkBox.Top = layerCheckBoxTop;
                layerCheckBoxTop += 20;
                checkBox.CheckedChanged += (s, e1) =>
                {
                    axMap1.set_LayerVisible(_initedMap.LayersInfo[checkBox.Text], checkBox.Checked);
                };
                layersManager.Controls.Add(checkBox);
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is IFocusable treeNode)
            {
                treeNode.Focus();
            }
            if (e.Node is INodeWithDescription nodeWithDescription)
            {
                entityDesc.Text = nodeWithDescription.GetDescription();
            }
        }

        private T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        private async void addExperimentBtn_Click(object sender, EventArgs e)
        {
            var form = new ExperimentForm(EditMode.Add);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var entity = form.Entity;

                var repository = GetService<IExperimentsRepository>();
                await repository.AddAsync(entity);
                if (await repository.SaveChanges())
                {
                    var node = new ExperimentTreeNode(entity);
                    node.SetMap(_initedMap);
                    treeView1.Nodes.Add(node);
                }
            }
        }

        private async void refresh_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            _initedMap = MapInitializer.Init(axMap1);
            var nodes = await new MapObjectsTreeBuilder().BuidNodes(new BuildNodesParams()
            {
                Map = _initedMap,
                ShowExperiments = true,
                ServiceProvider = _serviceProvider
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
        }
    }
}
