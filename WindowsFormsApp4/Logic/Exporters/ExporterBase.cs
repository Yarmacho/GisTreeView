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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Logic.Exporters
{
    internal abstract class ExporterBase
    {
        private readonly GeoDbContext _geoDbContext;
        private readonly IExperimentsRepository _experimentsRepository;
        private readonly IRoutePointsRepository _routePointsRepository;

        private string _battimetriesPath;

        protected ExporterBase()
        {
            _geoDbContext = Program.ServiceProvider.GetRequiredService<GeoDbContext>();
            _experimentsRepository = Program.ServiceProvider.GetRequiredService<IExperimentsRepository>();
            _routePointsRepository = Program.ServiceProvider.GetRequiredService<IRoutePointsRepository>();
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
                    Ships = new List<ShipDto>(),
                };

                sceneDto.Bathymetry = tryCreateBathymetryGrid(sceneDto);
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
                    Name = route.Name ?? $"Route {route.Id}",
                    Points = new List<RoutePointDto>()
                };

                await tryAddPointsToRoute(route.Id, routeDto);

                ship.Routes.Add(routeDto);
            }
        }
        private async ValueTask tryAddPointsToRoute(int routeId, RouteDto route)
        {
            var points = await _routePointsRepository.GetAllAsync(routeId);
            if (points == null || points.Count == 0)
            {
                return;
            }

            route.Points = points.Select(p => new RoutePointDto() 
            {
                RouteId = routeId,
                X = p.X,
                Y = p.Y,
                Depth = p.Depth,
                Salinity = p.Salinity,
                Speed = p.Speed,
                Temperature = p.Temperature,
                TimeOffset = p.TimeOffset
            }).ToList();
        }

        private Stream tryCreateBathymetryGrid(SceneDto scene)
        {
            var sceneBatimetryPath = Path.Combine(_battimetriesPath, $"Scene_{scene.Id}.asc");
            if (!Directory.Exists(_battimetriesPath) ||
                !File.Exists(sceneBatimetryPath))
            {
                return Stream.Null;
            }

            return File.OpenRead(sceneBatimetryPath);
        }
    }
}
