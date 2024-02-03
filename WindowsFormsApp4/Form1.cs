using Interfaces.Database.Repositories;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private const string _path = @"C:\Users\Yarmak Dmytro\Downloads\HydroPackege\Maps";

        private int _gasLayerHandle;
        private int _shipLayerHandle;
        private int _sceneLayerHandle;
        private int _profilLayerHandle;
        private int _tracesLayerHandle;

        public Form1(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(_path))
            {
                try
                {
                    axMap1.Projection = tkMapProjection.PROJECTION_CUSTOM;
                    axMap1.RemoveAllLayers();
                    axMap1.LockWindow(tkLockMode.lmLock);
                    foreach (var file in Directory.EnumerateFiles(_path))
                    {
                        int layerHandle = -1;
                        switch (Path.GetExtension(file).ToLowerInvariant())
                        {
                            case ".shp":
                                var shapeFile = new Shapefile();
                                if (shapeFile.Open(file))
                                {
                                    layerHandle = axMap1.AddLayer(shapeFile, true);
                                    shapeFile.Save();
                                }

                                switch (Path.GetFileNameWithoutExtension(file))
                                {
                                    case "Gas":
                                        _gasLayerHandle = layerHandle;
                                        break;
                                    case "Scene":
                                        _sceneLayerHandle = layerHandle;
                                        break;
                                    case "Ship":
                                        _shipLayerHandle = layerHandle;
                                        break;
                                    case "Profil":
                                        _profilLayerHandle = layerHandle;
                                        break;
                                }

                                break;
                            case ".tif":
                                var image = new Image();
                                if (image.Open(file, ImageType.TIFF_FILE))
                                {
                                    var colorScheme = new ColorScheme();
                                    colorScheme.SetColors2(tkMapColor.BlueViolet, tkMapColor.Blue);

                                    var greedColorScheme = new GridColorScheme();
                                    greedColorScheme.ApplyColors(tkColorSchemeType.ctSchemeGraduated, colorScheme, false);

                                    image.CustomColorScheme = greedColorScheme;
                                    axMap1.AddLayer(image, true);

                                    axMap1.GeoProjection = image.GeoProjection;
                                }
                                break;
                        }
                    }
                }
                finally
                {
                    axMap1.LockWindow(tkLockMode.lmUnlock);
                    axMap1.Redraw();
                }

                var nodes = await new MapObjectsTreeBuilder(_serviceProvider).BuidNodes(new BuildNodesParams() 
                { 
                    Map = axMap1,
                    GasLayerHandle = _gasLayerHandle,
                    ProfileLayerHandle = _profilLayerHandle,
                    SceneLayerHandle = _sceneLayerHandle,
                    ShipLayerHandle = _shipLayerHandle,
                    ShowExperiments = true
                });
                treeView1.Nodes.AddRange(nodes.ToArray());
                treeView1.AfterSelect += TreeView1_AfterSelect;

                var menuItems = new MenuItem[]
                {
                    new MenuItem("Add Experiment", async (s, e1) => 
                    {
                        var repository = GetService<IExperimentsRepository>();
                        var entity = await repository.AddAsync(new Entities.Experiment()
                        {
                            Name = "Test",
                            Description = "some desc"
                        });

                        if (await repository.SaveChanges())
                        {
                            treeView1.Nodes.Add(new TreeNode(entity.Name));
                        }
                    })
                };
                treeView1.ContextMenu = new ContextMenu(menuItems);

                axMap1.MouseDownEvent += AxMap1_MouseDownEvent;
            }
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
