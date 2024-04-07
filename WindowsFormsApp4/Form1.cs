using DynamicForms;
using DynamicForms.Factories;
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
using GeoDatabase.ORM;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _path;

        public Form1(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Text = "Experiments manager";
            _serviceProvider = serviceProvider;
            _path = configuration.GetValue<string>("MapsPath");
            InitializeComponent();
            treeView1.Map = axMap1;
            treeView1.ServiceProvider = _serviceProvider;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            axMap1.CursorMode = tkCursorMode.cmPan;
            axMap1.SendMouseMove = true;
            var initResult = MapInitializer.Init(_path, axMap1);
            MapDesigner.ConnectShipsWithGases(axMap1);
            if (initResult.BatimetryLayerHandle != -1)
            {
                var batimetry = axMap1.get_Image(initResult.BatimetryLayerHandle);
                if (batimetry != null)
                {
                    var band = batimetry.Band[1];

                    axMap1.MouseMoveEvent += (s, e1) =>
                    {
                        var longtitude = 0d;
                        var latitude = 0d;

                        axMap1.PixelToProj(e1.x, e1.y, ref longtitude, ref latitude);
                        batimetry.ProjectionToImage(longtitude, latitude, out int column, out int row);

                        var hasValue = band.Value[column, row, out var depthValue];
                        depth.AutoSize = true;
                        depth.Text = hasValue ? $"Depth: {depthValue}" : "Depth: undefined";
                    };
                }
            }
            if (initResult.SceneLayerHandle != -1)
            {
                axMap1.set_ShapeLayerFillTransparency(initResult.SceneLayerHandle, 0.3f);
            }
            treeView1.LayersInfo = initResult;
            var gasShp = axMap1.get_Shapefile(initResult.GasLayerHandle);

            var nodes = await new MapObjectsTreeBuilder().BuidNodes(new BuildNodesParams()
            {
                Map = axMap1,
                GasLayerHandle = initResult.GasLayerHandle,
                ProfileLayerHandle = initResult.ProfilLayerHandle,
                SceneLayerHandle = initResult.SceneLayerHandle,
                ShipLayerHandle = initResult.ShipLayerHandle,
                RoutesLayerHandle = initResult.RoutesLayerHandle,
                ShowExperiments = true,
                ServiceProvider = _serviceProvider
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
            treeView1.AfterSelect += TreeView1_AfterSelect;

            var layerCheckBoxTop = 0;
            foreach (var layer in initResult)
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
                    axMap1.set_LayerVisible(initResult[checkBox.Text], checkBox.Checked);
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
            var form = FormFactory.CreateForm<Experiment>(EditMode.Add);
            if (form.Activate() == DialogResult.OK)
            {
                var entity = form.GetEntity<Experiment>();

                var repository = GetService<IExperimentsRepository>();
                await repository.AddAsync(entity);
                if (await repository.SaveChanges())
                {
                    var node = new ExperimentTreeNode(entity, GetService<IRepositoriesProvider>());
                    node.SetMap(axMap1);
                    treeView1.Nodes.Add(node);
                }
            }
        }

        private async void refresh_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            var nodes = await new MapObjectsTreeBuilder().BuidNodes(new BuildNodesParams()
            {
                Map = axMap1,
                GasLayerHandle = treeView1.LayersInfo.GasLayerHandle,
                ProfileLayerHandle = treeView1.LayersInfo.ProfilLayerHandle,
                SceneLayerHandle = treeView1.LayersInfo.SceneLayerHandle,
                ShipLayerHandle = treeView1.LayersInfo.ShipLayerHandle,
                RoutesLayerHandle = treeView1.LayersInfo.RoutesLayerHandle,
                ShowExperiments = true,
                ServiceProvider = _serviceProvider
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
        }
    }
}
