using Entities;
using Entities.Entities;
using Interfaces.Database.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Logic.Exporters;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ExperimentTreeNode : EntityTreeNode<Experiment>
    {
        public int ExperimentId => Entity.Id;

        public ExperimentTreeNode(Experiment entity) : base(entity)
        {
            Name = entity.Name;
            Text = entity.Name;
            ImageKey = "experiment";
            SelectedImageKey = "experiment";
        }

        protected override void OnUpdate(Experiment entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            menu.MenuItems.Add(0, new MenuItem("Add scene", async (s, e) => await AppendChild<Scene, SceneTreeNode>()));

            var exportMenuItem = new MenuItem("Export");
            exportMenuItem.MenuItems.Add(new MenuItem("json", exportJsonAsync));
            exportMenuItem.MenuItems.Add(new MenuItem("xml", exportXmlAsync));

            menu.MenuItems.Add(exportMenuItem);

            return menu;
        }

        private async void exportJsonAsync(object sender, EventArgs eventArgs)
        {
            await exportToFile(new ExperimentJsonExporter(), "json");
        }

        private async void exportXmlAsync(object sender, EventArgs eventArgs)
        {
            await exportToFile(new ExperimentXmlExporter(), "xml");
        }

        private async Task exportToFile(ExporterBase exporter, string extension)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return;
                }

                var fileName = Path.Combine(fbd.SelectedPath, $"Experiment#{ExperimentId}.{extension}");

                await exporter.ExportAsync(ExperimentId, fileName);
            }
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Gas gas)
            {
                gas.ExperimentId = ExperimentId;
            }
            else if (childEntity is Scene scene)
            {
                scene.ExperimentId = ExperimentId;
            }
        }
    }
}
