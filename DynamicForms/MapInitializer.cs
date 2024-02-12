using AxMapWinGIS;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.IO;

namespace DynamicForms
{
    public static class MapInitializer
    {
        public static MapInitResult Init(string path, AxMap map)
        {
            var result = new MapInitResult();
            if (Directory.Exists(path))
            {
                try
                {
                    map.Projection = tkMapProjection.PROJECTION_CUSTOM;
                    map.RemoveAllLayers();
                    map.LockWindow(tkLockMode.lmLock);
                    foreach (var file in Directory.EnumerateFiles(path))
                    {
                        int layerHandle = -1;
                        switch (Path.GetExtension(file).ToLowerInvariant())
                        {
                            case ".shp":
                                var shapeFile = new Shapefile();
                                if (shapeFile.Open(file))
                                {
                                    layerHandle = map.AddLayer(shapeFile, true);
                                    shapeFile.Save();
                                }
                                result[Path.GetFileNameWithoutExtension(file)] = layerHandle;

                                break;
                            case ".tif":
                                var image = new Image();
                                if (image.Open(file, ImageType.TIFF_FILE))
                                {
                                    var colorScheme = new ColorScheme();
                                    colorScheme.SetColors2(tkMapColor.BlueViolet, tkMapColor.Blue);

                                    var greedColorScheme = new GridColorScheme();
                                    greedColorScheme.ApplyColors(tkColorSchemeType.ctSchemeGraduated, colorScheme, false);

                                    image.CustomColorScheme = greedColorScheme;
                                    map.AddLayer(image, true);

                                    map.GeoProjection = image.GeoProjection;
                                }
                                break;
                        }
                    }
                }
                finally
                {
                    map.LockWindow(tkLockMode.lmUnlock);
                    map.Redraw();
                }
            }

            return result;
        }
    }

    public class MapInitResult : Dictionary<string, int>
    {
        public MapInitResult() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public int GasLayerHandle => TryGetValue("Gas", out var layer) ? layer : -1;

        public int SceneLayerHandle => TryGetValue("Scene", out var layer) ? layer : -1;

        public int ShipLayerHandle => TryGetValue("Ship", out var layer) ? layer : -1;

        public int ProfilLayerHandle => TryGetValue("Profil", out var layer) ? layer : -1;

        public int RoutesLayerHadnle => TryGetValue("TraceLine", out var layer) ? layer : -1;
        public int CoastLayerHadnle => TryGetValue("BaseChine", out var layer) ? layer : -1;
    }
}
