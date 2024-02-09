using Entities.Entities;
using MapWinGIS;
using System;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class RoutePointTreeNode : ShapeTreeNode<RoutePoint>
    {
        public RoutePointTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
            : base(shapefile, shapeIndex, layerHandle)
        {
        }

        protected override ContextMenu BuildContextMenu()
        {
            return new ContextMenu();
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate(RoutePoint entity)
        {
            throw new NotImplementedException();
        }
    }
}
