using MassTransit;
using WindowsFormsApp4;
using System.Threading.Tasks;
using Tools;
using GeoDatabase.ORM;
using Entities.Entities;
using GeoDatabase.ORM.Set.Extensions;
using System;

namespace Consumers.Consumers.SceneCreated
{
    public class InterpolateBattimetryConsumer : IConsumer<Entities.Contracts.SceneCreated>
    {
        private readonly GeoDbContext _dbContext;

        public InterpolateBattimetryConsumer(GeoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<Entities.Contracts.SceneCreated> context)
        {
            var scene = _dbContext.Set<Scene>()
                .FirstOrDefault(s => s.Id == context.Message.SceneId);
            if (scene == null)
            {
                Console.WriteLine($"Scene {context.Message.SceneId} not found to build the battimetry");
                return Task.CompletedTask;
            }

            BattimetryInterpolator.CreateBatimetryIDW(Program.MainForm.Map, scene.Shape, scene.Id);

            NotificationsManager.Popup("Battimetry for scene interpolated");
            return Task.CompletedTask;
        }
    }
}
