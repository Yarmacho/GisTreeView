using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class SceneTreeNode : ShapeTreeNode
    {
        public SceneTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
            : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["SceneName"];
            if (nameFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
            Text = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
        }

        public IReadOnlyList<ShipTreeNode> ShipNodes => Nodes.OfType<ShipTreeNode>().ToList();

        public void AddNode(ShipTreeNode node)
        {
            Nodes.Add(node);
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();
            //menu.MenuItems.Add(new MenuItem("Add sea object", (s, e) => AppendChild<>));

            return menu;
        }
    }
}
