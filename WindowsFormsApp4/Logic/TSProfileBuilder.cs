using AxMapWinGIS;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.Logic
{
    public class TSProfileBuilder
    {
        private readonly Grid _bathymetryGrid;
        private readonly AxMap _map;
        private readonly int _blockSize = 100;
        private Grid _currentTSGrid;
        private readonly object _gridLock = new object();

        // Кешування для оптимізації
        private readonly Dictionary<ProfileType, Dictionary<int, Grid>> _gridCache;
        private const int MAX_CACHE_SIZE = 20;

        public int? LayerHandle = null;

        public TSProfileBuilder(Grid bathymetryGrid, AxMap map)
        {
            _bathymetryGrid = bathymetryGrid;
            _map = map;
            _gridCache = new Dictionary<ProfileType, Dictionary<int, Grid>>();
        }

        public void UpdateProfile(int depth, ProfileType profileType, CancellationToken cancellationToken)
        {
            try
            {
                // Перевіряємо кеш
                if (_gridCache.TryGetValue(profileType, out var profileCache) &&
                    profileCache.TryGetValue(depth, out Grid cachedGrid))
                {
                    UpdateMapLayer(cachedGrid, profileType);
                    return;
                }

                var newGrid = GenerateProfileGrid(depth, profileType, cancellationToken);

                lock (_gridLock)
                {
                    // Оновлюємо поточний грід
                    _currentTSGrid?.Close();
                    if (File.Exists(_currentTSGrid?.Filename))
                    {
                        File.Delete(_currentTSGrid.Filename);
                    }

                    _currentTSGrid = newGrid;

                    // Додаємо до кешу
                    ManageCache(depth, newGrid, profileType);
                }

                UpdateMapLayer(newGrid, profileType);
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка при оновленні профілю", ex);
            }
        }

        private Grid GenerateProfileGrid(int depth, ProfileType profileType, CancellationToken cancellationToken)
        {
            var tsGrid = new Grid();
            var gridFileName = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(_bathymetryGrid.Filename));

            var originalGrid = new Grid();
            originalGrid.Open(_bathymetryGrid.Filename);

            var header = new GridHeader();
            header.CopyFrom(originalGrid.Header);
            tsGrid.CreateNew(gridFileName, header,
                           GridDataType.FloatDataType, originalGrid.Header.NodataValue);

            CopyGridParameters(originalGrid, tsGrid);

            var (startCol, endCol, startRow, endRow) = GetGridIndicesFromExtents(_map.Extents);

            // Паралельна обробка блоків
            var loopResult = Parallel.For(startRow, endRow + 1, y =>
            {
                try
                {
                    for (int x = startCol; x <= endCol; x++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        double bathyDepth = Math.Abs((double)originalGrid.Value[y, x]);

                        if (bathyDepth >= depth &&
                            Math.Abs(bathyDepth - Convert.ToDouble(originalGrid.Header.NodataValue)) > double.Epsilon)
                        {
                            var profileValue = CalculateProfileValue(depth, bathyDepth, profileType);
                            tsGrid.Value[y, x] = profileValue;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    originalGrid.Close();
                    if (File.Exists(originalGrid.Filename))
                    {
                        File.Delete(originalGrid.Filename);
                    }
                }
            });

            while (!loopResult.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            return tsGrid;
        }

        private (int startCol, int endCol, int startRow, int endRow) GetGridIndicesFromExtents(Extents mapExtents)
        {
            // Конвертуємо координати екстентів у індекси сітки
            double xllCenter = _bathymetryGrid.Header.XllCenter;
            double yllCenter = _bathymetryGrid.Header.YllCenter;
            double cellSize = _bathymetryGrid.Header.dX;

            int startCol = Math.Max(0, (int)((mapExtents.xMin - xllCenter) / cellSize));
            int endCol = Math.Min(_bathymetryGrid.Header.NumberCols - 1,
                                (int)((mapExtents.xMax - xllCenter) / cellSize));

            int startRow = Math.Max(0, (int)((mapExtents.yMin - yllCenter) / cellSize));
            int endRow = Math.Min(_bathymetryGrid.Header.NumberRows - 1,
                                (int)((mapExtents.yMax - yllCenter) / cellSize));

            return (startCol, endCol, startRow, endRow);
        }

        private double CalculateProfileValue(int depth, double bathyDepth, ProfileType profileType)
        {
            switch (profileType)
            {
                case ProfileType.Temperature:
                    return CalculateTemperature(depth, bathyDepth);
                case ProfileType.Salinity:
                    return CalculateSalinity(depth, bathyDepth);
            }

            throw new ArgumentException("Невідомий тип профілю");
        }

        private double CalculateTemperature(int depth, double bathyDepth)
        {
            // Базова функція для температури (може бути модифікована)
            double surfaceTemp = 25.0;
            double bottomTemp = 2.0;
            double thermoclineDepth = 200.0;
            double thermoclineWidth = 100.0;

            if (depth < thermoclineDepth)
            {
                return surfaceTemp;
            }
            else if (depth > thermoclineDepth + thermoclineWidth)
            {
                return bottomTemp;
            }
            else
            {
                // Плавний перехід у термокліні
                double ratio = (depth - thermoclineDepth) / thermoclineWidth;
                return surfaceTemp + (bottomTemp - surfaceTemp) * ratio;
            }
        }

        private double CalculateSalinity(int depth, double bathyDepth)
        {
            // Базова функція для солоності (може бути модифікована)
            double surfaceSalinity = 35.0;
            double bottomSalinity = 34.5;
            double haloclineDepth = 150.0;
            double haloclineWidth = 100.0;

            if (depth < haloclineDepth)
            {
                return surfaceSalinity;
            }
            else if (depth > haloclineDepth + haloclineWidth)
            {
                return bottomSalinity;
            }
            else
            {
                // Плавний перехід у галокліні
                double ratio = (depth - haloclineDepth) / haloclineWidth;
                return surfaceSalinity + (bottomSalinity - surfaceSalinity) * ratio;
            }
        }

        private void UpdateMapLayer(Grid grid, ProfileType profileType)
        {
            if (LayerHandle.HasValue)
            {
                _map.RemoveLayer(LayerHandle.Value);
            }

            LayerHandle = _map.AddLayer(grid.OpenAsImage(CreateColorScheme(profileType)), true);
            _map.Redraw();
        }

        private void ManageCache(int depth, Grid grid, ProfileType profileType)
        {
            if (!_gridCache.TryGetValue(profileType, out var profileCache))
            {
                profileCache = new Dictionary<int, Grid>();
                _gridCache[profileType] = profileCache;
            }

            if (profileCache.Count >= MAX_CACHE_SIZE)
            {
                // Видаляємо найстаріший елемент
                var oldestDepth = profileCache.Keys.Min();
                profileCache[oldestDepth].Close();
                profileCache.Remove(oldestDepth);
            }

            profileCache[depth] = grid;
        }

        private void CopyGridParameters(Grid source, Grid target)
        {
            target.Header.XllCenter = source.Header.XllCenter;
            target.Header.YllCenter = source.Header.YllCenter;
            target.Header.dX = source.Header.dX;
            target.Header.dY = source.Header.dY;
            target.Header.Projection = source.Header.Projection;
            target.Header.GeoProjection = source.Header.GeoProjection;
            target.Header.NodataValue = source.Header.NodataValue;
        }

        private GridColorScheme CreateColorScheme(ProfileType profile)
        {
            return profile == ProfileType.Temperature
                ? getTemperatureColorScheme()
                : getSalinityColorScheme();
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

        public void Dispose()
        {
            lock (_gridLock)
            {
                _currentTSGrid?.Close();
                foreach (var grid in _gridCache.Values.SelectMany(v => v.Values))
                {
                    grid.Close();
                    if (File.Exists(grid.Filename))
                    {
                        File.Delete(grid.Filename);
                    }
                }
                _gridCache.Clear();
            }
        }
    }

    public enum ProfileType
    {
        Temperature,
        Salinity
    }
}
