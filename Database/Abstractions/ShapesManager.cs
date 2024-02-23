using Interfaces.Database.Abstractions;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Abstractions
{
    internal class ShapesManager : IShapesManager
    {
        public void DeleteAllShapes(string shapesPath)
        {
            if (!Directory.Exists(shapesPath))
            {
                return;
            }

            foreach (var file in Directory.EnumerateFiles(shapesPath, "*.shp"))
            {
                var shapefile = new Shapefile();
                if (!shapefile.Open(file))
                {
                    continue;
                }

                shapefile.StartEditingShapes();
                for (var i = 0; i < shapefile.NumShapes; i++)
                {
                    shapefile.EditDeleteShape(0);
                }
                shapefile.StopEditingShapes();
            }
        }
    }
}
