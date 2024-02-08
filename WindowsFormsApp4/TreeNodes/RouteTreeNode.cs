using MapWinGIS;
using System;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class RouteTreeNode : ShapeTreeNode
    {
        public RouteTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
            : base(shapefile, shapeIndex, layerHandle)
        {
            var idFieldIndex = Shapefile.FieldIndexByName["Id"];
            if (idFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = $"Trace {Shapefile.CellValue[idFieldIndex, shapeIndex]}";
            Text = $"Trace {Shapefile.CellValue[idFieldIndex, shapeIndex]}";
        }
    }
}
