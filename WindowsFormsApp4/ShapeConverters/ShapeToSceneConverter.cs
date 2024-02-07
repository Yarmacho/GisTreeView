using Entities.Entities;
using MapWinGIS;
using Tools;

namespace WindowsFormsApp4.ShapeConverters
{
    internal class ShapeToSceneConverter : IShapeEntityConverter<Scene>
    {
        public Scene FromShapeFile(Shapefile shapefile, int shapeIndex)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["SceneName"];
            var idFieldIndex = shapefile.FieldIndexByName["SceneId"];
            var gasIdFieldIndex = shapefile.FieldIndexByName["GasId"];

            return new Scene()
            {
                Id = TypeTools.Convert<int>(shapefile.CellValue[idFieldIndex, shapeIndex]),
                Name = TypeTools.Convert<string>(shapefile.CellValue[nameFieldIndex, shapeIndex]),
                GasId = TypeTools.Convert<int>(shapefile.CellValue[gasIdFieldIndex, shapeIndex]),
            };
        }

        public void WriteToShapeFile(Shapefile shapefile, int shapeIndex, Scene entity)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["SceneName"];
            var idFieldIndex = shapefile.FieldIndexByName["SceneId"];
            var gasIdFieldIndex = shapefile.FieldIndexByName["GasId"];

            shapefile.StartEditingShapes();

            shapefile.EditCellValue(nameFieldIndex, shapeIndex, entity.Name);
            shapefile.EditCellValue(gasIdFieldIndex, shapeIndex, entity.GasId);
            shapefile.EditCellValue(idFieldIndex, shapeIndex, entity.Id);

            shapefile.StopEditingShapes();
        }
    }
}
