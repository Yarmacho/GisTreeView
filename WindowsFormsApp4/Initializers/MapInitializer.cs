using AxMapWinGIS;
using DynamicForms;
using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApp4.Initializers
{
    public static class MapInitializer
    {
        internal static string ShapesPath { get; set; }

        public static Map Init(AxMap map, bool initSceneBattimetry = true)
        {
            var result = new MapInitResult();
            var sceneBattimetries = new Dictionary<int, int>();
            if (Directory.Exists(ShapesPath))
            {
                try
                {
                    map.Projection = tkMapProjection.PROJECTION_CUSTOM;
                    map.RemoveAllLayers();
                    map.LockWindow(tkLockMode.lmLock);
                    foreach (var file in Directory.EnumerateFiles(ShapesPath))
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
                                    layerHandle = map.AddLayer(image, true);

                                    map.GeoProjection = image.GeoProjection;
                                    map.ZoomToLayer(layerHandle);
                                    result[Path.GetFileNameWithoutExtension(file)] = layerHandle;
                                }
                                break;
                        }
                    }

                    if (initSceneBattimetry)
                    {
                        int layerHandle = -1;
                        var battimetriesPath = Path.Combine(ShapesPath, "Battimetries");
                        if (Directory.Exists(battimetriesPath))
                        {
                            foreach (var file in Directory.EnumerateFiles(battimetriesPath))
                            {
                                var image = new Image();
                                if (image.Open(file, ImageType.ASC_FILE))
                                {
                                    layerHandle = map.AddLayer(image, false);

                                    var fileWithoutExtension = Path.GetFileNameWithoutExtension(file);
                                    var slashIndex = fileWithoutExtension.IndexOf('_');
                                    if (slashIndex > 0 && int.TryParse(fileWithoutExtension.Substring(slashIndex + 1), out var sceneId))
                                    {
                                        sceneBattimetries.Add(sceneId, layerHandle);
                                    }
                                }
                            }
                        }
                    }

                    if (result.SceneLayerHandle != -1)
                    {
                        map.set_ShapeLayerFillTransparency(result.SceneLayerHandle, 0.3f);
                    }
                }
                finally
                {
                    map.LockWindow(tkLockMode.lmUnlock);
                    map.Redraw();
                }
            }

            var initedMap = new Map(map, result)
            {
                SceneBattimetries = sceneBattimetries
            };
            MapDesigner.ConnectShipsWithGases(initedMap);

            return initedMap;
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

        public int RoutesLayerHandle => TryGetValue("TraceLine", out var layer) ? layer : -1;
        public int CoastLayerHandle => TryGetValue("BaseChine", out var layer) ? layer : -1;
        public int BatimetryLayerHandle => TryGetValue("Batimetry", out var layer) ? layer : -1;

        public int GetLayerHandle(Type type) => TryGetValue(type.Name, out var layer) ? layer : -1;
        public int SetLayerHandle(Type type, int layerHandle) => this[type.Name] = layerHandle;

        public int GetLayerHandle<T>()
        {
            var entityType = typeof(T);
            if (entityType == typeof(Gas))
            {
                return GasLayerHandle;
            }
            else if (entityType == typeof(Ship))
            {
                return ShipLayerHandle;
            }
            else if (entityType == typeof(Scene))
            {
                return SceneLayerHandle;
            }
            else if (entityType == typeof(Route))
            {
                return RoutesLayerHandle;
            }

            throw new NotImplementedException();
        }
    }

    public class Map : IDisposable
    {
        public AxMap AxMap { get; }

        public MapInitResult LayersInfo { get; }

        public Dictionary<int, int> SceneBattimetries { get; set; }

        public Map(AxMap axMap, MapInitResult layersInfo)
        {
            AxMap = axMap;
            LayersInfo = layersInfo;
        }

        public void Redraw()
        {
            try
            {
                AxMap.LockWindow(tkLockMode.lmLock);
                AxMap.Redraw();
            }
            finally
            {
                AxMap.LockWindow(tkLockMode.lmUnlock);
            }
        }

        public tkCursorMode CursorMode
        {
            get => AxMap.CursorMode;
            set => AxMap.CursorMode = value;
        }

        public bool SendMouseMove
        {
            get => AxMap.SendMouseMove;
            set => AxMap.SendMouseMove = value;
        }

        public void PixelToProj(double x, double y, ref double projX, ref double projY)
            => AxMap.PixelToProj(x, y, ref projX, ref projY);

        public void Dispose()
        {
            AxMap?.Dispose();
        }

        public Shapefile GasShapeFile => AxMap.get_Shapefile(LayersInfo.GasLayerHandle);
        public Shapefile ShipShapeFile => AxMap.get_Shapefile(LayersInfo.ShipLayerHandle);
        public Shapefile SceneShapeFile => AxMap.get_Shapefile(LayersInfo.SceneLayerHandle);
        public Shapefile RoutesShapeFile => AxMap.get_Shapefile(LayersInfo.RoutesLayerHandle);
        public Shapefile CoastShapeFile => AxMap.get_Shapefile(LayersInfo.CoastLayerHandle);
        public Image Batimetry => AxMap.get_Image(LayersInfo.BatimetryLayerHandle);

        public void ZoomToLayer<T>()
        {
            AxMap.ZoomToLayer(LayersInfo.GetLayerHandle<T>());
        }

        public void ZoomToShape<T>(int shapeIndex)
        {
            AxMap.ZoomToShape(LayersInfo.GetLayerHandle<T>(), shapeIndex);
        }

        // <ShipId, DrawingId>
        public Dictionary<int, int> ShipGasLinesIndexes = new Dictionary<int, int>();
    }
}
