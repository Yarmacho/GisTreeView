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
    internal class GasTreeNode : ShapeTreeNode<Gas, int>
    {
        public GasTreeNode(Gas gas) : base(gas)
        {
            Name = gas.Name;
            Text = gas.Name;
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
                scene.GasId = Entity.Id;
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
