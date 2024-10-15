using Entities.Entities;
using MapWinGIS;
using System;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class RouteTreeNode : ShapeTreeNode<Route, int>
    {
        public RouteTreeNode(Route route) : base(route)
        {
        }

        internal void SetRoute(Route route)
        {
            Name = route.Name;
            Text = route.Name;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
        }

        protected override void OnUpdate(Route entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
