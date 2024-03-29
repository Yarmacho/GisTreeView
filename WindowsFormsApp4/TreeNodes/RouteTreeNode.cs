﻿using Entities.Entities;
using MapWinGIS;
using System;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class RouteTreeNode : ShapeTreeNode<Route>
    {
        public RouteTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
            : base(shapefile, shapeIndex, layerHandle)
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
