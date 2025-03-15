using Entities.Contracts;
using MapWinGIS;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Logic;

namespace WindowsFormsApp4.Events.Handlers.Scenes
{
    internal class CalculateProfilesConsumer : EventHandlerBase<ProfilesRequested>
    {
        public override ValueTask Handle(ProfilesRequested state)
        {
            var battimetryFileName = Path.Combine(Program.Configuration.GetValue<string>("MapsPath"),
                Program.Configuration.GetValue<string>("BattimetryFileName"));

            var batimetryGrid = new Grid();
            if (!batimetryGrid.Open(battimetryFileName))
            {
                MessageBox.Show("Battimetry not found");
                return new ValueTask();
            }

            var outputFolder = Path.Combine(Program.Configuration.GetValue<string>("MapsPath"),
                Program.Configuration.GetValue<string>("ProfilesFolder"), "Map");

            try
            {
                var tsProcessor = new TsTableProcessor(batimetryGrid, outputFolder);
                tsProcessor.GenerateTsGrids();
                MessageBox.Show("Profiles calculated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return new ValueTask();
        }
    }
}
