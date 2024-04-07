using AxMapWinGIS;
using Entities.Entities;
using GeoDatabase.ORM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;

namespace DynamicForms
{
    public static class MapDesigner
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void ConnectShipsWithGases(AxMap map)
        {
            var dbContext = ServiceProvider.GetRequiredService<GeoDbContext>();

            var ships = dbContext.Set<Ship>().ToList();
            var scenesShips = ships.ToLookup(s => s.SceneId);

            var sceneIds = scenesShips.Select(s => s.Key).ToList();
            var gasScenes = dbContext.Set<Scene>().Where(s => sceneIds.Contains(s.Id))
                .ToDictionary(s => s.GasId, s => s.Id);

            var gases = dbContext.Set<Gas>().Where(g => gasScenes.Keys.Contains(g.Id)).ToList();

            foreach (var gas in gases)
            {
                if (!gasScenes.TryGetValue(gas.Id, out var sceneId))
                {
                    continue;
                }

                var sceneShips = scenesShips[sceneId];
                if (!sceneShips.Any())
                {
                    continue;
                }

                var gasPoint = gas.Shape.Point[0];
                foreach (var ship in sceneShips)
                {
                    var shipPoint = ship.Shape.Point[0];

                    map.NewDrawing(MapWinGIS.tkDrawReferenceList.dlSpatiallyReferencedList);
                    map.DrawLine(gasPoint.x, gasPoint.y, shipPoint.x, shipPoint.y, 2, (uint)Color.Red.ToArgb());
                }
            }
        }

        public static void ConnectShipWithGas(AxMap map, Ship ship)
        {
            var dbContext = ServiceProvider.GetRequiredService<GeoDbContext>();

            var scene = dbContext.Set<Scene>().FirstOrDefault(s => s.Id == ship.SceneId);
            if (scene == null)
            {
                throw new Exception("Scene not found");
            }

            var gas = dbContext.Set<Gas>().FirstOrDefault(g => g.Id == scene.GasId);
            if (gas == null)
            {
                throw new Exception("Gas not found");
            }

            var gasPoint = gas.Shape.Point[0];
            var shipPoint = ship.Shape.Point[0];

            map.NewDrawing(MapWinGIS.tkDrawReferenceList.dlSpatiallyReferencedList);
            map.DrawLine(gasPoint.x, gasPoint.y, shipPoint.x, shipPoint.y, 2, (uint)Color.Red.ToArgb());
        }
    }
}
