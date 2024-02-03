using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    class ShipTreeNode : ShapeTreeNode
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
    }
}
