using MapWinGIS;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private const string _path = @"C:\Users\Yarmak Dmytro\Desktop\Maps";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(_path))
            {
                //var file = Path.Combine(_path, "Gas.shp");
                //var shapeFile = new Shapefile();
                //if (shapeFile.Open(file))
                //{
                //    for (int i = 0; i < shapeFile.NumShapes; i++)
                //    {
                //        var node = new GasTreeNode(shapeFile, i);
                //        treeView1.Nodes.Add(node);

                //        node.SetMap(axMap1);
                //        treeView1.AfterSelect += TreeView1_AfterSelect;
                //    }
                //}
                var buildParams = new BuildNodesParams() { Map = axMap1 };
                try
                {
                    axMap1.Projection = tkMapProjection.PROJECTION_CUSTOM;
                    axMap1.GrabProjectionFromData = true;
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
                                    layerHandle = axMap1.AddLayerFromFilename(file, tkFileOpenStrategy.fosAutoDetect, false);
                                    //axMap1.AddLayer(shapeFile, false);
                                    //shapeFile.Save();
                                }

                                switch (Path.GetFileNameWithoutExtension(file))
                                {
                                    case "Gas":
                                        buildParams.GasLayerHandle = layerHandle;
                                        break;
                                    case "Scene":
                                        buildParams.SceneLayerHandle = layerHandle;
                                        break;
                                    case "Ship":
                                        buildParams.ShipLayerHandle = layerHandle;
                                        break;
                                    case "Profil":
                                        buildParams.ProfileLayerHandle = layerHandle;
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

                var nodes = new MapObjectsTreeBuilder().BuidNodes(buildParams);
                treeView1.Nodes.AddRange(nodes.ToArray());
                treeView1.AfterSelect += TreeView1_AfterSelect;
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is MapTreeNode treeNode)
            {
                treeNode.Focus();
            }
        }
    }
}
