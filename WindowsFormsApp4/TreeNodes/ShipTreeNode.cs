using Entities.Entities;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    class ShipTreeNode : ShapeTreeNode<Ship, int>
    {
        public ShipTreeNode(Ship ship) : base(ship)
        {
            Name = ship.Name;
            Text = ship.Name;
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            menu.MenuItems.Add(0, new MenuItem("Add route", async (s, e) => await AppendChild<Route, RouteTreeNode>()));

            return menu;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Route route)
            {
                route.ShipId = Entity.Id;
            }
        }

        protected override void OnUpdate(Ship entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
