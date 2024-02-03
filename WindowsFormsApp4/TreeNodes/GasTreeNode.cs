using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class GasTreeNode : ShapeTreeNode
    {
        public GasTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle) : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["Type"];
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
    }
}
