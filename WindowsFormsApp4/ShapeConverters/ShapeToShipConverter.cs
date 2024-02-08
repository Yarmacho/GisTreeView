using Entities.Entities;
using MapWinGIS;
using System;
using Tools;

namespace WindowsFormsApp4.ShapeConverters
{
    internal class ShapeToShipConverter : IShapeEntityConverter<Ship>
    {
        public Ship FromShapeFile(Shapefile shapefile, int shapeIndex)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["Name_sh"];
            var idFieldIndex = shapefile.FieldIndexByName["ShipId"];
            var sceneIdFieldIndex = shapefile.FieldIndexByName["SceneId"];

            return new Ship()
            {
                Id = TypeTools.Convert<int>(shapefile.CellValue[idFieldIndex, shapeIndex]),
                Name = TypeTools.Convert<string>(shapefile.CellValue[nameFieldIndex, shapeIndex]),
                SceneId = TypeTools.Convert<int>(shapefile.CellValue[sceneIdFieldIndex, shapeIndex]),
            };
        }

        public void WriteToShapeFile(Shapefile shapefile, int shapeIndex, Ship entity)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["Name_sh"];
            var idFieldIndex = shapefile.FieldIndexByName["ShipId"];
            var sceneIdFieldIndex = shapefile.FieldIndexByName["SceneId"];

            shapefile.StartEditingShapes();

            shapefile.EditCellValue(nameFieldIndex, shapeIndex, entity.Name);
            shapefile.EditCellValue(sceneIdFieldIndex, shapeIndex, entity.SceneId);
            shapefile.EditCellValue(idFieldIndex, shapeIndex, entity.Id);

            shapefile.StopEditingShapes();
        }
    }
}
