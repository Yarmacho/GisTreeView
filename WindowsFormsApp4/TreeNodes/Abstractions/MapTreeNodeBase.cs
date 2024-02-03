using AxMapWinGIS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class MapTreeNodeBase : TreeNode
    {
        protected AxMap Map;

        public virtual bool AppendMode { get; set; }
        public string AppendModeKey { get; protected set; }

        public abstract ValueTask Delete();

        protected virtual ContextMenu BuildContextMenu()
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Delete", (s, e) => Delete()),
            };

            return new ContextMenu(items.ToArray());
        }

        public void SetMap(AxMap map)
        {
            Map = map;
            foreach (var node in Nodes.OfType<ShapeTreeNode>())
            {
                node.SetMap(map);
            }
        }
    }
}
