using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace WindowsFormsApp4.ShapeConverters
{
    internal class ShapeToRouteConverter : IShapeEntityConverter<Route>
    {
        public Route FromShapeFile(Shapefile shapefile, int shapeIndex)
        {
            var idFieldIndex = shapefile.FieldIndexByName["Id"];

            return new Route()
            {
                Id = TypeTools.Convert<int>(shapefile.CellValue[idFieldIndex, shapeIndex]),
            };
        }

        public void WriteToShapeFile(Shapefile shapefile, int shapeIndex, Route entity)
        {
            var idFieldIndex = shapefile.FieldIndexByName["Id"];

            shapefile.StartEditingShapes();

            shapefile.EditCellValue(idFieldIndex, shapeIndex, entity.Id);

            shapefile.StopEditingShapes();
        }
    }
}
