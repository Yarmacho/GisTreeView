using AxMapWinGIS;
using MapWinGIS;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp4.TreeNodes
{
    internal abstract class MapTreeNode : TreeNode
    {
        protected readonly Shapefile Shapefile;
        protected readonly int ShapeIndex;
        protected readonly int LayerHandle;

        private AxMap _map;

        public MapTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
        {
            Shapefile = shapefile;
            ShapeIndex = shapeIndex;
            LayerHandle = layerHandle;

            ContextMenu = BuildContextMenu();
        }

        public void Delete()
        {
            Shapefile.StartEditingShapes();
            if (Shapefile.EditDeleteShape(ShapeIndex) && Shapefile.Save())
            {
                Remove();
            }
            Shapefile.StopEditingShapes();
        }

        public void Focus()
        {
            _map.ZoomToShape(LayerHandle, ShapeIndex);
        }

        public void SetMap(AxMap map)
        {
            _map = map;
            foreach (var node in Nodes.OfType<MapTreeNode>())
            {
                node.SetMap(map);
            }
        }

        protected virtual ContextMenu BuildContextMenu()
        {
            var items = new MenuItem[]
            {
                new MenuItem("Delete", (s, e) => Delete())
            };

            return new ContextMenu(items);
        }
    }
}
      