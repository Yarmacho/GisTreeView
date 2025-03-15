using Entities.Dtos;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApp4.Logic.Exporters
{
    internal class ExperimentXmlExporter : ExporterBase
    {
        public override async Task<bool> ExportAsync(int experimentId, string outputFileName, CancellationToken cancellationToken = default)
        {
            var experiment = await CreateExperimentAsync(experimentId, cancellationToken);

            return saveToFileAsync(experiment, outputFileName, cancellationToken);
        }

        private bool saveToFileAsync(ExperimentDto experiment, string outputFileName, CancellationToken cancellationToken)
        {
            try
            {
                var serializer = new ExperimentXmlSerializer();
                using (var fileStream = File.Open(outputFileName, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(fileStream, experiment);
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
