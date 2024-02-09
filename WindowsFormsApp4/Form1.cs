using DynamicForms;
using Tools.Attributes;
using DynamicForms.Factories;
using Entities;
using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes;
using WindowsFormsApp4.TreeNodes.Abstractions;
using Tools;

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
            treeView1.Map = axMap1;
            treeView1.ServiceProvider = _serviceProvider;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            axMap1.CursorMode = tkCursorMode.cmPan;
            var initResult = MapInitializer.Init(_path, axMap1);
            treeView1.LayersInfo = initResult;

            var nodes = await new MapObjectsTreeBuilder().BuidNodes(new BuildNodesParams()
            {
                Map = axMap1,
                GasLayerHandle = initResult.GasLayerHandle,
                ProfileLayerHandle = initResult.ProfilLayerHandle,
                SceneLayerHandle = initResult.SceneLayerHandle,
                ShipLayerHandle = initResult.ShipLayerHandle,
                RoutesLayerHandle = initResult.RoutesLayerHadnle,
                ShowExperiments = true,
                ServiceProvider = _serviceProvider
            });
            treeView1.Nodes.AddRange(nodes.ToArray());
            treeView1.AfterSelect += TreeView1_AfterSelect;

            axMap1.MouseDownEvent += AxMap1_MouseDownEvent;
        }

        private void AxMap1_MouseDownEvent(object sender, AxMapWinGIS._DMapEvents_MouseDownEvent e)
        {
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is IFocusable treeNode)
            {
                treeNode.Focus();
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
    }
}
