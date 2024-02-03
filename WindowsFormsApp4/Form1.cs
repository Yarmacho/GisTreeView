using DynamicForms;
using DynamicForms.Attributes;
using DynamicForms.Factories;
using Entities;
using MapWinGIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _path;

        public Form1(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _path = configuration.GetValue<string>("MapsPath");
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var initResult = MapInitializer.Init(_path, axMap1);

            var nodes = await new MapObjectsTreeBuilder(_serviceProvider).BuidNodes(new BuildNodesParams()
            {
                Map = axMap1,
                GasLayerHandle = initResult.GasLayerHandle,
                ProfileLayerHandle = initResult.ProfilLayerHandle,
                SceneLayerHandle = initResult.SceneLayerHandle,
                ShipLayerHandle = initResult.ShipLayerHandle,
                ShowExperiments = true
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
            treeView1.AfterSelect += TreeView1_AfterSelect;

            var menuItems = new MenuItem[]
            {
                    new MenuItem("Add Experiment", async (s, e1) =>
                    {
                        var form = FormFactory.CreateFormWithMap(new Experiment()
                        {
                            Name = "My test exp",
                            Description = "some desc"
                        }, _path, ShpfileType.SHP_POINT, EditMode.Add);

                        if (form.Activate() == DialogResult.OK)
                        {
                            var newExp = form.GetEntity<Experiment>();
                        }
                    })
            };
            treeView1.ContextMenu = new ContextMenu(menuItems);

            axMap1.MouseDownEvent += AxMap1_MouseDownEvent;
        }

        private void AxMap1_MouseDownEvent(object sender, AxMapWinGIS._DMapEvents_MouseDownEvent e)
        {
            if (treeView1.SelectedNode is ShapeTreeNode mapTreeNode &&
                mapTreeNode.AppendMode)
            {
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is ShapeTreeNode treeNode)
            {
                treeNode.Focus();
            }
        }

        private T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
