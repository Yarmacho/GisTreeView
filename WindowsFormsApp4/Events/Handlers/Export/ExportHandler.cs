using Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Logic.Exporters;

namespace WindowsFormsApp4.Events.Handlers.Export
{
    internal class ExportHandler : EventHandlerBase<ExportRequested>
    {
        public override async ValueTask Handle(ExportRequested state)
        {
            ExporterBase exporter = null;
            switch (state.ExportType)
            {
                case "json":
                    exporter = new ExperimentJsonExporter();
                    break;
                case "xml":
                    exporter = new ExperimentXmlExporter();
                    break;
                default: 
                    throw new NotImplementedException();
            }

            await exporter.ExportAsync(state.ExperimentId, state.OutputFile);

            MessageBox.Show(Program.MainForm, $"Experiment {state.ExperimentId} exported to {state.OutputFile}");
        }
    }
}
