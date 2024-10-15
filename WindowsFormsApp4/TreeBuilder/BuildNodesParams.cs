using AxMapWinGIS;
using System;
using System.Collections.Generic;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4
{
    public class BuildNodesParams
    {
        public Map Map { get; set; }

        public int GasLayerHandle => Map.LayersInfo.GasLayerHandle;

        public int ShipLayerHandle => Map.LayersInfo.ShipLayerHandle;

        public int SceneLayerHandle => Map.LayersInfo.SceneLayerHandle;

        //public int ProfileLayerHandle => Map.LayersInfo.ProfileLayerHandle;

        public int RoutesLayerHandle => Map.LayersInfo.RoutesLayerHandle;

        public bool ShowExperiments { get; set; } = true;

        public IEnumerable<int> ExperimentIds { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
