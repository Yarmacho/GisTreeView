using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class SceneTreeNode : ShapeTreeNode<Scene>
    {
        public SceneTreeNode(Scene scene, int shapeIndex, int layerHandle)
            : base(scene, shapeIndex, layerHandle)
        {
            Name = scene.Name;
            Text = scene.Name;
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
                ship.SceneId = Entity.Id;
            }
        }

        protected override void OnUpdate(Scene entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
