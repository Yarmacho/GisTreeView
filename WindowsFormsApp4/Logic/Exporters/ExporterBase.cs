using Entities;
using Entities.Dtos;
using Entities.Entities;
using GeoDatabase.ORM;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MassTransit.Util.ChartTable;

namespace WindowsFormsApp4.Logic.Exporters
{
    internal abstract class ExporterBase
    {
        private readonly GeoDbContext _geoDbContext;
        private readonly IExperimentsRepository _experimentsRepository;

        private string _battimetriesPath;

        protected ExporterBase()
        {
            _geoDbContext = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
            _experimentsRepository = Program.ServiceProvider.GetRequiredService<IExperimentsRepository>();
            _battimetriesPath = Path.Combine(Program.Configuration.GetValue<string>("MapsPath"), "Battimetries");
        }

        public abstract Task<bool> ExportAsync(int experimentId, string outputFileName, CancellationToken cancellationToken = default);

        protected async Task<ExperimentDto> CreateExperimentAsync(int experimentId, CancellationToken cancellationToken)
        {
            var experiment = await _experimentsRepository.GetByIdAsync(experimentId, cancellationToken);
            if (experiment == null)
            {
                NotificationsManager.Popup("Experiment not found");
                return null;
            }

            var experimentDto = new ExperimentDto()
            {
                Id = experimentId,
                Description = experiment.Description,
                Name = experiment.Name,
                Scenes = new List<SceneDto>()
            };

            await tryAddScenesToExperiment(experimentDto);

            return experimentDto;
        }

        private async ValueTask tryAddScenesToExperiment(ExperimentDto experiment)
        {
            var scenes = _geoDbContext.Set<Scene>()
                .Where(s => s.ExperimentId == experiment.Id)
                .ToList();

            if (scenes == null || scenes.Count == 0)
            {
                return;
            }

            foreach (var scene in scenes)
            {
                var sceneDto = new SceneDto()
                {
                    Id = scene.Id,
                    Angle = scene.Angle,
                    Area = scene.Area,
                    ExperimentId = experiment.Id,
                    Name = scene.Name,
                    Side = scene.Side,
                    Sensors = new List<GasDto>(),
                    Ships = new List<ShipDto>()
                };

                await tryAddSensorsToScene(sceneDto);
                await tryAddShipsToScene(sceneDto);

                experiment.Scenes.Add(sceneDto);
            }
        }

        private ValueTask tryAddSensorsToScene(SceneDto scene)
        {
            var sensors = _geoDbContext.Set<Gas>()
                .Where(s => s.SceneId == scene.Id)
                .ToList();

            if (sensors == null || sensors.Count == 0)
            {
                return new ValueTask();
            }

            foreach (var sensor in sensors)
            {
                var sensorDto = new GasDto()
                {
                    Id = sensor.Id,
                    SceneId = sensor.SceneId,
                    Name = sensor.Name,
                    X = sensor.X,
                    Y = sensor.Y,
                };

                scene.Sensors.Add(sensorDto);
            }
            return new ValueTask();
        }

        private async ValueTask tryAddShipsToScene(SceneDto scene)
        {
            var ships = _geoDbContext.Set<Ship>()
                .Where(s => s.SceneId == scene.Id)
                .ToList();

            if (ships == null || ships.Count == 0)
            {
                return;
            }

            foreach (var ship in ships)
            {
                var shipDto = new ShipDto()
                {
                    Id = ship.Id,
                    SceneId = ship.SceneId,
                    Name = ship.Name,
                    X = ship.X,
                    Y = ship.Y,
                    Acceleration = ship.Acceleration,
                    Deceleration = ship.Deceleration,
                    Lenght = ship.Lenght,
                    MaxSpeed = ship.MaxSpeed,
                    TurnRate = ship.TurnRate,
                    Width = ship.Width,
                    Routes = new List<RouteDto>()
                };

                await tryAddRoutesToShip(shipDto);

                scene.Ships.Add(shipDto);
            }
        }

        private async ValueTask tryAddRoutesToShip(ShipDto ship)
        {
            var routes = _geoDbContext.Set<Route>()
                .Where(s => s.ShipId == ship.Id)
                .ToList();

            if (routes == null || routes.Count == 0)
            {
                return;
            }

            foreach (var route in routes)
            {
                var routeDto = new RouteDto()
                {
                    ShipId = route.ShipId,
                    Description = route.Description,
                    Name = route.Name,
                    Points = new List<RoutePointDto>()
                };

                await tryAddPointsToRoute(routeDto);

                ship.Routes.Add(routeDto);
            }
        }
        private ValueTask tryAddPointsToRoute(RouteDto route)
        {
            return new ValueTask();
        }

        private async ValueTask<BathymetryGrid> tryCreateBathymetryGrid(SceneDto scene)
        {
            var grid = new BathymetryGrid();

            var path = Path.Combine(_battimetriesPath, $"Scene_{scene.Id}.asc");
            if (!File.Exists(path))
            {
                return grid;
            }

            var columns = 0;
            var rows = 0;
            var cellsize = 0d;
            var xllcorner = 0d;
            var yllcorner = 0d;
            using (var reader = File.OpenText(path))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (Regex.IsMatch("^[a-zA-Z]+\\s+\\-?[0-9,;]+$", line))
                    {
                        var splitedLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        var key = splitedLine[0];
                        var value = splitedLine[1];
                        grid.Metadata.Add(key, value);

                        switch (key)
                        {
                            case "ncols":
                                columns = int.Parse(value);
                                break;
                            case "nrows":
                                rows = int.Parse(value);
                                break;
                            case "cellsize":
                                cellsize = double.Parse(value);
                                break;
                            case "xllcorner":
                                xllcorner = double.Parse(value);
                                break;
                            case "yllcorner":
                                yllcorner = double.Parse(value);
                                break;
                        }
                        continue;
                    }

                    var bathymetryValues = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var row = 0;
                    var col = 0;
                    foreach (var value in bathymetryValues)
                    {
                        var depth = double.Parse(value);

                        //var point = new BathymetryPoint
                        //{
                        //    X = xllcorner + (col * cellsize),
                        //    Y = cornerLatitude + (row * cellsize),
                        //    Depth = depth,
                        //    XRangeStart = xllcorner + (col * cellsize) - (cellsize / 2),
                        //    XRangeEnd = xllcorner + (col * cellsize) + (cellsize / 2),
                        //    YRangeStart = cornerLatitude + (row * cellsize) - (cellsize / 2),
                        //    YRangeEnd = cornerLatitude + (row * cellsize) + (cellsize / 2)
                        //};

                        //grid.Points.Add(point);
                    }
                }
            }

            return grid;
        }
    }
}
