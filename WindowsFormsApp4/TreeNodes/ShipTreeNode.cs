using DynamicForms;
using Entities.Entities;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    class ShipTreeNode : ShapeTreeNode<Ship>
    {
        public ShipTreeNode(Ship ship, int shapeIndex, int layerHandle)
            : base(ship, shapeIndex, layerHandle)
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

        internal override ValueTask OnAppend(Ship entity)
        {
            MapDesigner.ConnectShipWithGas(Map, entity);

            return new ValueTask();
        }
    }
}
