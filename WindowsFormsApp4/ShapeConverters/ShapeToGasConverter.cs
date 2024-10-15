using Entities.Entities;
using MapWinGIS;
using Tools;

namespace WindowsFormsApp4.ShapeConverters
{
    internal class ShapeToGasConverter : IShapeEntityConverter<Gas>
    {
        public Gas FromShapeFile(Shapefile shapefile, int shapeIndex)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["Ent_num"];
            var idFieldIndex = shapefile.FieldIndexByName["Id"];
            var experimentIdFieldIndex = shapefile.FieldIndexByName["Experiment"];

            return new Gas()
            {
                Id = TypeTools.Convert<int>(shapefile.CellValue[idFieldIndex, shapeIndex]),
                Name = TypeTools.Convert<string>(shapefile.CellValue[nameFieldIndex, shapeIndex]),
                ExperimentId = TypeTools.Convert<int>(shapefile.CellValue[experimentIdFieldIndex, shapeIndex]),
            };
        }

        public void WriteToShapeFile(Shapefile shapefile, int shapeIndex, Gas entity)
        {
            var nameFieldIndex = shapefile.FieldIndexByName["Ent_num"];
            var idFieldIndex = shapefile.FieldIndexByName["Id"];
            var experimentIdFieldIndex = shapefile.FieldIndexByName["Experiment"];

            shapefile.StartEditingShapes();

            shapefile.EditCellValue(nameFieldIndex, shapeIndex, entity.Name);
            shapefile.EditCellValue(experimentIdFieldIndex, shapeIndex, entity.ExperimentId);
            shapefile.EditCellValue(idFieldIndex, shapeIndex, entity.Id);

            shapefile.StopEditingShapes();
        }
    }
}
