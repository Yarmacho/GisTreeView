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
    internal class GasTreeNode : ShapeTreeNode<Gas>
    {
        public GasTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle) : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["Ent_num"];
            if (nameFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
            Text = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
        }

        public IReadOnlyList<SceneTreeNode> SceneNodes => Nodes.OfType<SceneTreeNode>().ToList();

        public void AddNode(SceneTreeNode node)
        {
            Nodes.Add(node);
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Scene scene)
            {
                var idFieldIndex = Shapefile.FieldIndexByName["Id"];
                if (idFieldIndex != -1)
                {
                    scene.GasId = TypeTools.Convert<int>(Shapefile.CellValue[idFieldIndex, ShapeIndex]);
                }
            }
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            var addBtn = new MenuItem("Add scene", async (s, e) =>
            {
                if (Nodes.Count > 0)
                {
                    MessageBox.Show("Only one scene can be created for a GAS");
                    return;
                }
                await AppendChild<Scene, SceneTreeNode>();
            });

            menu.MenuItems.Add(0, addBtn);

            return menu;
        }

        protected override void OnUpdate(Gas entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
