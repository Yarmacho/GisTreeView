using AxMapWinGIS;
using DynamicForms;
using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Image = MapWinGIS.Image;

namespace WindowsFormsApp4.Initializers
{
    public static class MapInitializer
    {
        internal static string ShapesPath { get; set; }

        internal static string TemperatureProfileFileName { get; set; }
        internal static string SalnityProfileFileName { get; set; }
        internal static string CoastFileName { get; set; }

        public static Map Init(AxMap map, bool initSceneBattimetry = true, bool initSalnity = false,
            bool initTemperature = false)
        {
            var result = new MapInitResult(TemperatureProfileFileName, SalnityProfileFileName);
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
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                if ((!initTemperature && fileName == TemperatureProfileFileName) ||
                                    (!initSalnity && fileName == SalnityProfileFileName))
                                {
                                    continue;
                                }

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

        public static Map InitBattimetries(AxMap map, bool initSceneBattimetry = true, bool initSalnity = false,
            bool initTemperature = false)
        {
            var result = new MapInitResult(TemperatureProfileFileName, SalnityProfileFileName);
            var sceneBattimetries = new Dictionary<int, int>();

            if (Directory.Exists(ShapesPath))
            {
                try
                {
                    map.Projection = tkMapProjection.PROJECTION_CUSTOM;
                    map.RemoveAllLayers();
                    map.LockWindow(tkLockMode.lmLock);

                    var coastFileName = Path.Combine(ShapesPath, CoastFileName);
                    if (File.Exists(coastFileName))
                    {
                        var layerHandle = -1;
                        var shapeFile = new Shapefile();
                        if (shapeFile.Open(coastFileName))
                        {
                            layerHandle = map.AddLayer(shapeFile, true);
                            shapeFile.Save();
                        }
                        result[Path.GetFileNameWithoutExtension(coastFileName)] = layerHandle;
                    }

                    foreach (var file in Directory.EnumerateFiles(ShapesPath, "*.tif"))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);

                        if ((!initTemperature && fileName == TemperatureProfileFileName) ||
                            (!initSalnity && fileName == SalnityProfileFileName))
                        {
                            continue;
                        }

                        int layerHandle = -1;
                        switch (Path.GetExtension(file).ToLowerInvariant())
                        {
                            case ".tif":
                                var image = new Image();
                                if (image.Open(file, ImageType.TIFF_FILE))
                                {
                                    var colorScheme = new ColorScheme();
                                    colorScheme.SetColors2(tkMapColor.BlueViolet, tkMapColor.Blue);

                                    var greedColorScheme = new GridColorScheme();
                                    greedColorScheme.ApplyColors(tkColorSchemeType.ctSchemeGraduated, colorScheme, false);

                                    image.CustomColorScheme = fileName == TemperatureProfileFileName 
                                        ? getTemperatureColorScheme()
                                        : fileName == SalnityProfileFileName 
                                            ? getSalinityColorScheme()
                                            : greedColorScheme;

                                    if (fileName == TemperatureProfileFileName ||
                                        fileName == SalnityProfileFileName)
                                    {
                                        image.UseTransparencyColor = true;
                                        image.TransparencyColor = ColorToUint32(Color.White);
                                        image.TransparencyPercent = 50;
                                        image.UpsamplingMode = tkInterpolationMode.imHighQualityBicubic;
                                    }
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
            return initedMap;
        }

        private static GridColorScheme getTemperatureColorScheme()
        {
            var colorScheme = new GridColorScheme();

            // Налаштування діапазонів температур та відповідних кольорів
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = -9999,
                HighValue = -9999,
                LowColor = ColorToUint32(Color.Transparent),
                HighColor = ColorToUint32(Color.Transparent)
            });
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = -5d,
                LowColor = ColorToUint32(Color.FromArgb(0, 0, 255)),
                HighColor = ColorToUint32(Color.FromArgb(0, 128, 255)),
                HighValue = 0
            });
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 0,
                LowColor = ColorToUint32(Color.FromArgb(0, 128, 255)),
                HighColor = ColorToUint32(Color.FromArgb(0, 255, 255)),
                HighValue = 10
            });
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 20,
                LowColor = ColorToUint32(Color.FromArgb(0, 255, 255)),
                HighColor = ColorToUint32(Color.FromArgb(255, 128, 0)),
                HighValue = 25
            });
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 25,
                LowColor = ColorToUint32(Color.FromArgb(255, 128, 0)),
                HighColor = ColorToUint32(Color.FromArgb(255, 0, 0)),
                HighValue = 35
            });

            return colorScheme;
        }

        private static GridColorScheme getSalinityColorScheme()
        {
            var colorScheme = new GridColorScheme();

            // NoData значення
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = -9999,
                HighValue = -9999,
                LowColor = ColorToUint32(Color.Transparent),
                HighColor = ColorToUint32(Color.Transparent)
            });

            // Дуже низька солоність (прісна/солонувата вода)
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 0,
                LowColor = ColorToUint32(Color.FromArgb(240, 249, 255)),  // Дуже світло-блакитний
                HighColor = ColorToUint32(Color.FromArgb(198, 219, 239)), // Світло-блакитний
                HighValue = 20
            });

            // Низька солоність (перехідна зона)
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 20,
                LowColor = ColorToUint32(Color.FromArgb(198, 219, 239)),  // Світло-блакитний
                HighColor = ColorToUint32(Color.FromArgb(158, 202, 225)), // Середньо-блакитний
                HighValue = 30
            });

            // Середня солоність
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 30,
                LowColor = ColorToUint32(Color.FromArgb(158, 202, 225)),  // Середньо-блакитний
                HighColor = ColorToUint32(Color.FromArgb(107, 174, 214)), // Насичений блакитний
                HighValue = 34
            });

            // Підвищена солоність
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 34,
                LowColor = ColorToUint32(Color.FromArgb(107, 174, 214)),  // Насичений блакитний
                HighColor = ColorToUint32(Color.FromArgb(66, 146, 198)),  // Темно-блакитний
                HighValue = 37
            });

            // Висока солоність
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 37,
                LowColor = ColorToUint32(Color.FromArgb(66, 146, 198)),   // Темно-блакитний
                HighColor = ColorToUint32(Color.FromArgb(33, 113, 181)),  // Дуже темно-блакитний
                HighValue = 40
            });

            // Екстремально висока солоність
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = 40,
                LowColor = ColorToUint32(Color.FromArgb(33, 113, 181)),   // Дуже темно-блакитний
                HighColor = ColorToUint32(Color.FromArgb(8, 69, 148)),    // Найтемніший блакитний
                HighValue = 45
            });

            return colorScheme;
        }

        private static uint ColorToUint32(Color color)
        {
            return (uint)color.ToArgb();
        }
    }

    public class MapInitResult : Dictionary<string, int>
    {
        private readonly string _temperatureFileName;
        private readonly string _salnityFileName;

        public MapInitResult(string temperatureFileName, string salnityFileName) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            _temperatureFileName = temperatureFileName;
            _salnityFileName = salnityFileName;
        }

        public int GasLayerHandle => TryGetValue("Gas", out var layer) ? layer : -1;

        public int SceneLayerHandle => TryGetValue("Scene", out var layer) ? layer : -1;

        public int ShipLayerHandle => TryGetValue("Ship", out var layer) ? layer : -1;

        public int ProfilLayerHandle => TryGetValue("Profil", out var layer) ? layer : -1;

        public int RoutesLayerHandle => TryGetValue("TraceLine", out var layer) ? layer : -1;
        public int CoastLayerHandle => TryGetValue("BaseChine", out var layer) ? layer : -1;
        public int BatimetryLayerHandle => TryGetValue("Batimetry", out var layer) ? layer : -1;
        public int TemperatureLayerHandle => TryGetValue(_temperatureFileName, out var layer) ? layer : -1;
        public int SalnityLayerHandle => TryGetValue(_salnityFileName, out var layer) ? layer : -1;

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
        public Image Temperature => AxMap.get_Image(LayersInfo.TemperatureLayerHandle);
        public Image Salnity => AxMap.get_Image(LayersInfo.SalnityLayerHandle);

        public void ZoomToLayer<T>()
        {
            AxMap.ZoomToLayer(LayersInfo.GetLayerHandle<T>());
        }

        public void ZoomToShape<T>(int shapeIndex)
        {
            AxMap.ZoomToShape(LayersInfo.GetLayerHandle<T>(), shapeIndex);
        }

        // <<ShipId, GasId>, DrawingId>
        public Dictionary<ShipGasPair, int> ShipGasLinesIndexes = new Dictionary<ShipGasPair, int>();
    }

    public class ShipGasPair 
    {
        public int GasId { get; }

        public int ShipId { get; }

        public ShipGasPair(int gasId, int shipId)
        {
            GasId = gasId;
            ShipId = shipId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }

            if (ReferenceEquals(this, obj)) { return true; }

            if (obj.GetType() != this.GetType()) { return false; }

            ShipGasPair other = (ShipGasPair)obj;
            return GasId == other.GasId && ShipId == other.ShipId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int hashingBase = 397;
                int hash = 17;

                hash = hash * hashingBase + GasId.GetHashCode();
                hash = hash * hashingBase + ShipId.GetHashCode();

                return hash;
            }
        }
    }
}
