using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ProfilTreeNode : ShapeTreeNode
    {
        public ProfilTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle) 
            : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["Id"];
            if (nameFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = $"Profil {Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString()}";
            Text = $"Profil {Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString()}";
        }
    }
}
