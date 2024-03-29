﻿using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    class ShipTreeNode : ShapeTreeNode<Ship>
    {
        public ShipTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
            : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["Name_sh"];
            if (nameFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
            Text = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
        }

        public IReadOnlyList<ProfilTreeNode> ShipNodes => Nodes.OfType<ProfilTreeNode>().ToList();

        public void AddNode(ProfilTreeNode node)
        {
            Nodes.Add(node);
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
                var idFieldIndex = Shapefile.FieldIndexByName["ShipId"];
                if (idFieldIndex != -1)
                {
                    route.ShipId = TypeTools.Convert<int>(Shapefile.CellValue[idFieldIndex, ShapeIndex]);
                }
            }
        }

        protected override void OnUpdate(Ship entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
