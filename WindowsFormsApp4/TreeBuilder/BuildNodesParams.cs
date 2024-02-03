using AxMapWinGIS;
using System.Collections.Generic;

namespace WindowsFormsApp4
{
    public class BuildNodesParams
    {
        public AxMap Map { get; set; }

        public int GasLayerHandle { get; set; } = -1;

        public int ShipLayerHandle { get; set; } = -1;

        public int SceneLayerHandle { get; set; } = -1;

        public int ProfileLayerHandle { get; set; } = -1;

        public bool ShowExperiments { get; set; } = true;

        public IEnumerable<int> ExperimentIds { get; set; }
    }
}
