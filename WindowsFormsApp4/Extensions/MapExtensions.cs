using Entities.Entities;
using MapWinGIS;
using System;

namespace WindowsFormsApp4.Extensions
{
    internal static class MapExtensions
    {
        public static Shapefile GetEntityShapefile<T>(this Initializers.Map map)
        {
            var entityType = typeof(T);
            if (entityType == typeof(Gas))
            {
                return map.GasShapeFile;
            }
            else if (entityType == typeof(Ship))
            {
                return map.ShipShapeFile;
            }
            else if (entityType == typeof(Scene))
            {
                return map.SceneShapeFile;
            }
            else if (entityType == typeof(Route))
            {
                return map.RoutesShapeFile;
            }

            throw new NotImplementedException();
        }
    }
}
