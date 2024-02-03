using AxMapWinGIS;
using MapWinGIS;
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

                                switch (Path.GetFileNameWithoutExtension(file))
                                {
                                    case "Gas":
                                        result.GasLayerHandle = layerHandle;
                                        break;
                                    case "Scene":
                                        result.SceneLayerHandle = layerHandle;
                                        break;
                                    case "Ship":
                                        result.ShipLayerHandle = layerHandle;
                                        break;
                                    case "Profil":
                                        result.ProfilLayerHandle = layerHandle;
                                        break;
                                }

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

    public class MapInitResult
    {
        public int GasLayerHandle = -1;

        public int SceneLayerHandle = -1;

        public int ShipLayerHandle = -1;

        public int ProfilLayerHandle = -1;
    }
}
