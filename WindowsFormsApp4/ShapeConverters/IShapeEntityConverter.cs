using Entities;
using MapWinGIS;

namespace WindowsFormsApp4.ShapeConverters
{
    public interface IShapeEntityConverter<TEntity> where TEntity : EntityBase, new()
    {
        void WriteToShapeFile(Shapefile shapefile, int shapeIndex, TEntity entity);

        TEntity FromShapeFile(Shapefile shapefile, int shapeIndex);
    }
}
