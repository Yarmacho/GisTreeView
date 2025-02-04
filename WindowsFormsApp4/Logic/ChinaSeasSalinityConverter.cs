using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Logic
{
    internal class ChinaSeasSalinityConverter
    {
        private readonly Dictionary<int, (double minTemp, double maxTemp)> _depthTemperatures;
        private readonly Dictionary<int, (double minSalinity, double maxSalinity)> _depthSalinity;
        private readonly Random _random;
        private readonly ChineseRegion _region;
        private readonly Season _season;

        private readonly double _noData = -9999;

        public ChinaSeasSalinityConverter(ChineseRegion region = ChineseRegion.EastChinaSea,
            Season season = Season.Summer)
        {
            _random = new Random();
            _region = region;
            _season = season;
            _depthTemperatures = InitializeTemperatureData();
            _depthSalinity = InitializeSalinityData();
        }

        private Dictionary<int, (double minTemp, double maxTemp)> InitializeTemperatureData()
        {
            // Реальні дані температур для Східно-Китайського та Південно-Китайського морів
            switch (_region)
            {
                case ChineseRegion.EastChinaSea:
                    // Східно-Китайське море
                    return new Dictionary<int, (double, double)>
                    {
                        {0, GetSeasonalTemperature(28.0, 30.0)},    // Поверхня
                        {50, GetSeasonalTemperature(22.0, 24.0)},   // Верхній шар
                        {100, GetSeasonalTemperature(18.0, 20.0)},  // Середній шар
                        {200, (15.0, 17.0)},                        // Нижній шар
                        {500, (8.0, 10.0)},                         // Глибинний шар
                        {1000, (4.0, 6.0)}                          // Придонний шар
                    };

                case ChineseRegion.SouthChinaSea:
                default:
                    // Південно-Китайське море
                    return new Dictionary<int, (double, double)>
                    {
                        {0, GetSeasonalTemperature(29.0, 31.0)},    // Поверхня
                        {50, GetSeasonalTemperature(24.0, 26.0)},   // Верхній шар
                        {100, GetSeasonalTemperature(20.0, 22.0)},  // Середній шар
                        {200, (16.0, 18.0)},                        // Нижній шар
                        {500, (10.0, 12.0)},                        // Глибинний шар
                        {1000, (4.0, 6.0)},                         // Проміжний шар
                        {2000, (2.0, 3.0)}                          // Придонний шар
                    };
            }
        }

        private Dictionary<int, (double minSalinity, double maxSalinity)> InitializeSalinityData()
        {
            switch (_region)
            {
                case ChineseRegion.EastChinaSea:
                    // Східно-Китайське море характеризується нижчою солоністю через вплив річки Янцзи
                    return new Dictionary<int, (double, double)>
                    {
                        {0, (30.0, 32.0)},    // Поверхневий шар
                        {50, (32.0, 33.5)},   // Підповерхневий шар
                        {100, (33.5, 34.0)},  // Проміжний шар
                        {200, (34.0, 34.5)},  // Нижній шар
                        {500, (34.5, 34.8)},  // Глибинний шар
                        {1000, (34.8, 35.0)}  // Придонний шар
                    };

                case ChineseRegion.SouthChinaSea:
                default:
                    // Південно-Китайське море має вищу солоність
                    return new Dictionary<int, (double, double)>
                    {
                        {0, (33.0, 34.0)},    // Поверхневий шар
                        {50, (33.5, 34.5)},   // Підповерхневий шар
                        {100, (34.0, 34.8)},  // Проміжний шар
                        {200, (34.5, 35.0)},  // Нижній шар
                        {500, (34.8, 35.2)},  // Глибинний шар
                        {1000, (34.9, 35.3)}, // Проміжний глибинний шар
                        {2000, (35.0, 35.5)}  // Придонний шар
                    };
            }
        }

        private (double min, double max) GetSeasonalTemperature(double baseMin, double baseMax)
        {
            double adjustment = _season == Season.Winter
                ? -8d
                : _season == Season.Spring
                    ? -3d
                    : _season == Season.Autumn
                        ? -4d
                        : 0d;

            return (baseMin + adjustment, baseMax + adjustment);
        }

        public bool ConvertBathymetry(string bathymetryPath, string temperaturePath, string salinityPath)
        {
            try
            {
                var bathymetryGrid = new Grid();
                if (!bathymetryGrid.Open(bathymetryPath))
                {
                    throw new Exception("Не вдалося відкрити файл батиметрії");
                }

                // Створюємо сітки для температури та солоності
                var temperatureGrid = CreateGrid(bathymetryGrid, temperaturePath);
                var salinityGrid = CreateGrid(bathymetryGrid, salinityPath);

                var result = Parallel.For(0, bathymetryGrid.Header.NumberRows, y =>
                {
                    Parallel.For(0, bathymetryGrid.Header.NumberCols, x => 
                    {
                        double depth = (double)bathymetryGrid.Value[y, x];
                        temperatureGrid.Value[y, x] = CalculateTemperature(-depth);
                        salinityGrid.Value[y, x] = CalculateSalinity(-depth);
                    });
                });

                while (!result.IsCompleted)
                {
                    Thread.Sleep(1000);
                }
                //// Конвертуємо дані
                //for (int y = 0; y < bathymetryGrid.Header.NumberRows; y++)
                //{
                //    for (int x = 0; x < bathymetryGrid.Header.NumberCols; x++)
                //    {
                //        double depth = Math.Abs((double)bathymetryGrid.Value[y, x]);
                //        temperatureGrid.Value[y, x] = CalculateTemperature(depth);
                //        salinityGrid.Value[y, x] = CalculateSalinity(depth);
                //    }
                //}

                // Зберігаємо результати
                bool tempSaved = temperatureGrid.Save(temperaturePath, GridFileType.GeoTiff);
                bool salSaved = salinityGrid.Save(salinityPath, GridFileType.GeoTiff);

                bathymetryGrid.Close();
                temperatureGrid.Close();
                salinityGrid.Close();

                return tempSaved && salSaved;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при конвертації: {ex.Message}");
                return false;
            }
        }

        private Grid CreateGrid(Grid sourceGrid, string fileName)
        {
            var newGrid = new Grid();

            var header = new GridHeader();
            header.CopyFrom(sourceGrid.Header);
            header.NodataValue = _noData;

            newGrid.CreateNew(fileName, header,
                GridDataType.FloatDataType, _noData);

            return newGrid;
        }

        private double CalculateTemperature(double depth)
        {
            return CalculateValueFromDepth(depth, _depthTemperatures);
        }

        private double CalculateSalinity(double depth)
        {
            return CalculateValueFromDepth(depth, _depthSalinity);
        }

        private double CalculateValueFromDepth(double depth, Dictionary<int, (double minValue, double maxValue)> depthData)
        {
            if (depth < 0) return _noData;

            var depthLevels = new List<int>(depthData.Keys);
            depthLevels.Sort();

            // Поверхневі води
            if (depth <= depthLevels[0])
            {
                var values = depthData[depthLevels[0]];
                return InterpolateValue(values.minValue, values.maxValue);
            }

            // Глибинні води
            if (depth >= depthLevels[depthLevels.Count - 1])
            {
                var values = depthData[depthLevels[depthLevels.Count - 1]];
                return InterpolateValue(values.minValue, values.maxValue);
            }

            // Пошук інтервалу глибин
            for (int i = 0; i < depthLevels.Count - 1; i++)
            {
                if (depth >= depthLevels[i] && depth < depthLevels[i + 1])
                {
                    var lowerValues = depthData[depthLevels[i]];
                    var upperValues = depthData[depthLevels[i + 1]];
                    double ratio = (depth - depthLevels[i]) / (depthLevels[i + 1] - depthLevels[i]);

                    double minValue = Lerp(lowerValues.minValue, upperValues.minValue, ratio);
                    double maxValue = Lerp(lowerValues.maxValue, upperValues.maxValue, ratio);

                    return InterpolateValue(minValue, maxValue);
                }
            }

            return _noData;
        }

        private double Lerp(double start, double end, double ratio)
        {
            return start + (end - start) * ratio;
        }

        private double InterpolateValue(double min, double max)
        {
            return Math.Round(min + (_random.NextDouble() * (max - min)), 4);
        }
    }

    public enum ChineseRegion
    {
        EastChinaSea,    // Східно-Китайське море
        SouthChinaSea    // Південно-Китайське море
    }
}
