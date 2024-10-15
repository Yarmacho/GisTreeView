using System;

namespace GeoDatabase.ORM.Mapper
{
    public interface IShapesMapper
    {
        T Map<T>(int shapeIndex) where T : new();

        object Map(int shapeIndex, Type destinationType);
    }
}
