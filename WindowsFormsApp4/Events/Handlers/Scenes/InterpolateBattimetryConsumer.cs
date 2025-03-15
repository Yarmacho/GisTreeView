using MassTransit;
using WindowsFormsApp4;
using System.Threading.Tasks;
using Tools;
using GeoDatabase.ORM;
using Entities.Entities;
using GeoDatabase.ORM.Set.Extensions;
using System;
using WindowsFormsApp4.Events.Handlers;
using Entities.Contracts;
using System.Runtime.Remoting.Contexts;
using WindowsFormsApp4.Initializers;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Events.Handlers
{
    internal class InterpolateBattimetryHandler : EventHandlerBase<SceneCreated>
    {
        private readonly GeoDbContext _dbContext;

        public InterpolateBattimetryHandler(GeoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override ValueTask Handle(SceneCreated state)
        {
            var scene = _dbContext.Set<Scene>()
                .FirstOrDefault(s => s.Id == state.SceneId);
            if (scene == null)
            {
                Console.WriteLine($"Scene {state.SceneId} not found to build the battimetry");
                return new ValueTask();
            }

            var battimetry = new MapWinGIS.Image();
            if (!battimetry.Open(Program.MainForm.Map.Batimetry.Filename))
            {
                return new ValueTask();
            }

            var battimetryPath = BattimetryInterpolator.CreateBatimetryIDW(battimetry, scene.Shape,
                Program.MainForm.Map.AxMap.Width, scene.Id);

            var sceneBattimetry = new MapWinGIS.Image();
            sceneBattimetry.Open(battimetryPath, MapWinGIS.ImageType.ASC_FILE);

            var layerHandle = Program.MainForm.Map.AxMap.AddLayer(sceneBattimetry, false);
            Program.MainForm.Map.SceneBattimetries[scene.Id] = layerHandle;

            MessageBox.Show(Program.MainForm, $"Scene \"{scene.Name ?? scene.Id.ToString()}\"battimetry calculated");
            return new ValueTask();
        }
    }
}
