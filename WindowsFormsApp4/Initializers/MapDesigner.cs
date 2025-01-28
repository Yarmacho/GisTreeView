using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsFormsApp4.Initializers;

namespace DynamicForms
{
    public static class MapDesigner
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void ConnectShipsWithGases(Map map)
        {
            var dbContext = ServiceProvider.GetRequiredService<GeoDbContext>();

            var ships = dbContext.Set<Ship>().ToList();
            foreach (var ship in ships)
            {
                ConnectShipWithGases(map, ship);
            }
        }

        public static void ConnectShipWithGases(Map map, Ship ship)
        {
            var dbContext = ServiceProvider.GetRequiredService<GeoDbContext>();

            var scene = dbContext.Set<Scene>().FirstOrDefault(s => s.Id == ship.SceneId);
            if (scene == null)
            {
                throw new Exception("Scene not found");
            }

            var gases = dbContext.Set<Gas>().Where(g => g.SceneId == scene.Id).ToList();
            connectShipWithGas(map, ship, gases);
        }

        private static void connectShipWithGas(Map map, Ship ship, List<Gas> gases)
        {
            foreach (var gas in gases)
            {
                var gasPoint = gas.Shape.Point[0];
                var shipPoint = ship.Shape.Point[0];

                var key = new ShipGasPair(gas.Id, ship.Id);
                if (map.ShipGasLinesIndexes.TryGetValue(key, out var drawingIndex))
                {
                    map.AxMap.ClearDrawing(drawingIndex);
                }

                drawingIndex = map.AxMap.NewDrawing(MapWinGIS.tkDrawReferenceList.dlSpatiallyReferencedList);
                map.ShipGasLinesIndexes[key] = drawingIndex;

                map.AxMap.DrawLineEx(drawingIndex, gasPoint.x, gasPoint.y, shipPoint.x, shipPoint.y, 2, (uint)Color.Red.ToArgb());
            }
        }
    }
}