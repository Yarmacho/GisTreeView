namespace Entities.Contracts
{
    public class ExportRequested
    {
        public int ExperimentId { get; set; }

        public string ExportType { get; set; }

        public string OutputFile { get; set; }
    }
}
