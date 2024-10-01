using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class SceneTreeNode : ShapeTreeNode<Scene>
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
            menu.MenuItems.Add(0, new MenuItem("Add sea object", async (s, e) => await AppendChild<Ship, ShipTreeNode>()));

            return menu;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Ship ship)
            {
                var idFieldIndex = Shapefile.FieldIndexByName["SceneId"];
                if (idFieldIndex != -1)
                {
                    ship.SceneId = TypeTools.Convert<int>(Shapefile.CellValue[idFieldIndex, ShapeIndex]);
                }
            }
        }

        protected override void OnUpdate(Scene entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        public override async ValueTask<bool> AppendChild<TChildEntity, TChildNode>()
        {
            if (!await base.AppendChild<TChildEntity, TChildNode>())
            {
                return false;
            }

            var shape = Shapefile.Shape[ShapeIndex];
            if (shape is null)
            {
                return false;
            } 

            var battimetry = TreeView.Map.get_Image(TreeView.LayersInfo.BatimetryLayerHandle);
            if (battimetry is null)
            {
                return true;
            }

            return true;
        }
    }
}
