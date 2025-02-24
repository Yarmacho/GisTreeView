using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Logic
{
    public class TsTableProcessor
    {
        private readonly Grid _bathymetryGrid;
        private readonly int _maxDepth;
        private string _tsDataPath;
        private int _noDataValue;

        public TsTableProcessor(Grid bathymetryGrid, string outputPath)
        {
            _bathymetryGrid = bathymetryGrid;
            _tsDataPath = outputPath;

            var maxDepth = Convert.ToInt32(bathymetryGrid.Minimum);
            _maxDepth = Math.Abs(maxDepth > 0 ? 0 : maxDepth);

            _noDataValue = Convert.ToInt32(_bathymetryGrid.Header.NodataValue);
        }

        public void GenerateTsGrids()
        {
            var depthProfiles = new Dictionary<int, (StreamWriter temeratureProfile, StreamWriter salinityProfile)>();
            try
            {
                for (int depth = 1; depth <= _maxDepth; depth++)
                {
                    var temperatureGridPath = Path.Combine(_tsDataPath, $"temperature_{depth}.asc");
                    var salinityGridPath = Path.Combine(_tsDataPath, $"salinity_{depth}.asc");

                    var temperatureProfile = new StreamWriter(File.OpenWrite(temperatureGridPath));
                    writeHeaderInfo(temperatureProfile);
                    createProjectionFile(temperatureGridPath);

                    var salinityProfile = new StreamWriter(File.OpenWrite(salinityGridPath));
                    writeHeaderInfo(salinityProfile);
                    createProjectionFile(salinityGridPath);

                    depthProfiles.Add(depth, (temperatureProfile, salinityProfile));
                }

                int totalRows = _bathymetryGrid.Header.NumberRows;
                int totalCols = _bathymetryGrid.Header.NumberCols;

                var chunkSize = totalRows / 10;

                Enumerable.Range(0, totalRows).Select(row => Task.Run(() =>
                {
                    Parallel.For(0, totalCols, col =>
                    {
                        for (int depth = 1; depth < 1; depth++)
                        {
                            var currentDepthProfiles = depthProfiles[depth];
                            var tempProfile = currentDepthProfiles.temeratureProfile;
                            var salinityProfile = currentDepthProfiles.salinityProfile;

                            double bathymetryDepth = Convert.ToDouble(_bathymetryGrid.Value[row, col]);

                            
                            if (depth <= bathymetryDepth)
                            {
                                var (temperature, salinity) = CalculateTsValues(row, col, depth);

                                tempProfile.Write($"{temperature:F2} ");
                                salinityProfile.Write($"{salinity:F2} ");
                            }
                            else
                            {
                                tempProfile.Write($"{_bathymetryGrid.Header.NodataValue:F2} ");
                                salinityProfile.Write($"{_bathymetryGrid.Header.NodataValue:F2} ");
                            }

                            tempProfile.Flush();
                            salinityProfile.Flush();
                        }
                    });
                }));
            }
            finally
            {
                foreach (var profiles in depthProfiles.Values)
                {
                    profiles.temeratureProfile?.Dispose();
                    profiles.salinityProfile?.Dispose();
                }
            }
        }

        private void writeHeaderInfo(StreamWriter writer)
        {
            writer.WriteLine($"NCOLS {_bathymetryGrid.Header.NumberCols}");
            writer.WriteLine($"NROWS {_bathymetryGrid.Header.NumberRows}");
            writer.WriteLine($"XLLCENTER {_bathymetryGrid.Header.XllCenter}");
            writer.WriteLine($"YLLCENTER {_bathymetryGrid.Header.YllCenter}");
            writer.WriteLine($"CELLSIZE {_bathymetryGrid.Header.dX}");
            writer.WriteLine($"NODATA_VALUE {_bathymetryGrid.Header.NodataValue}");
        }

        private void createProjectionFile(string gridPath)
        {
            var projectionPath = Path.ChangeExtension(gridPath, ".prj");
            using (var writer = new StreamWriter(projectionPath))
            {
                writer.Write(_bathymetryGrid.Header.Projection);
            }
        }

        private (double temperature, double salinity) CalculateTsValues(int row, int col, int depth)
        {
            //// Конвертуємо індекси гріду в географічні координати
            //double x = _bathymetryGrid.Header.XllCenter + (col * _bathymetryGrid.Header.dX);
            //double y = _bathymetryGrid.Header.YllCenter + (row * _bathymetryGrid.Header.dY);

            // Тут має бути ваша логіка розрахунку температури та солоності
            // На основі x, y, depth та даних вимірювань
            double temperature = 20 - (depth * 0.5);
            double salinity = 35 + (depth * 0.1);

            return (temperature, salinity);
        }

        public MapWinGIS.Image LoadTsGrid(int depth, ProfileType profile)
        {
            var path = profile == ProfileType.Temperature
                ? Path.Combine(_tsDataPath, $"temperature_{depth}.asc") 
                : Path.Combine(_tsDataPath, $"salinity_{depth}.asc");

            var grid = new Grid();

            try
            {
                grid.Open(path);
                return grid.OpenAsImage(createColorScheme(profile));
            }
            catch (Exception ex)
            {
                return new MapWinGIS.Image();
            }
        }

        private GridColorScheme createColorScheme(ProfileType profile)
        {
            return profile == ProfileType.Temperature
                ? getTemperatureColorScheme()
                : getSalinityColorScheme();
        }

        private GridColorScheme getTemperatureColorScheme()
        {
            var colorScheme = new GridColorScheme();

            // Налаштування діапазонів температур та відповідних кольорів
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = _noDataValue,
                HighValue = _noDataValue,
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

        private GridColorScheme getSalinityColorScheme()
        {
            var colorScheme = new GridColorScheme();

            // NoData значення
            colorScheme.InsertBreak(new GridColorBreak()
            {
                ColoringType = ColoringType.Gradient,
                LowValue = _noDataValue,
                HighValue = _noDataValue,
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
}
