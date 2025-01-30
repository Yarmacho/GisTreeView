using Entities.Dtos;
using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Logic.Exporters
{
    internal class ExperimentJsonExporter : ExporterBase
    {
        public override async Task<bool> ExportAsync(int experimentId, string outputFileName, CancellationToken cancellationToken = default)
        {
            var experiment = await CreateExperimentAsync(experimentId, cancellationToken);

            return await saveToFileAsync(experiment, outputFileName, cancellationToken);
        }

        private async Task<bool> saveToFileAsync(ExperimentDto experiment, string outputFileName, CancellationToken cancellationToken)
        {
            try
            {
                using (var fileStream = File.Open(outputFileName, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fileStream, experiment, cancellationToken: cancellationToken);
                }

                return true;
            }
            catch
            {

                return false;
            }
        }
    }
}
