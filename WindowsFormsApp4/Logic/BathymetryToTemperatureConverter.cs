using MapWinGIS;
using System;
using System.Collections.Generic;

namespace WindowsFormsApp4.Logic
{
    public class BathymetryToTemperatureConverter
    {
        private readonly Dictionary<int, (double minTemp, double maxTemp)> _depthTemperatures;
        private readonly Random _random;
        private readonly double _seasonalAdjustment;

        public BathymetryToTemperatureConverter(Season season = Season.Summer)
        {
            _random = new Random();
            _seasonalAdjustment = GetSeasonalAdjustment(season);
            _depthTemperatures = InitializeTemperatureData();
        }

        private Dictionary<int, (double minTemp, double maxTemp)> InitializeTemperatureData()
        {
            return new Dictionary<int, (double, double)>
            {
                {0, (20.0 + _seasonalAdjustment, 30.0 + _seasonalAdjustment)},
                {50, (18.0 + _seasonalAdjustment * 0.8, 25.0 + _seasonalAdjustment * 0.8)},
                {100, (15.0 + _seasonalAdjustment * 0.6, 20.0 + _seasonalAdjustment * 0.6)},
                {200, (12.0 + _seasonalAdjustment * 0.4, 16.0 + _seasonalAdjustment * 0.4)},
                {500, (8.0 + _seasonalAdjustment * 0.2, 12.0 + _seasonalAdjustment * 0.2)},
                {1000, (4.0, 6.0)},
                {2000, (2.0, 4.0)},
                {4000, (1.0, 2.0)}
            };
        }

        private double GetSeasonalAdjustment(Season season)
        {
            return season == Season.Winter
                ? -5d
                : season == Season.Summer
                    ? 5d
                    : 0d;
        }

        public bool ConvertBathymetryToTemperature(string bathymetryPath, string outputPath)
        {
            try
            {
                var bathymetryGrid = new Grid();
                if (!bathymetryGrid.Open(bathymetryPath))
                {
                    throw new Exception("Не вдалося відкрити файл батиметрії");
                }

                var temperatureGrid = new Grid();
                temperatureGrid.CreateNew(outputPath, bathymetryGrid.Header,
                    GridDataType.FloatDataType, -9999);

                // Конвертуємо глибини в температури
                for (int y = 0; y < bathymetryGrid.Header.NumberRows; y++)
                {
                    for (int x = 0; x < bathymetryGrid.Header.NumberCols; x++)
                    {
                        double depth = Math.Abs((double)bathymetryGrid.Value[y, x]);
                        double temperature = CalculateTemperature(depth);
                        temperatureGrid.Value[y, x] = temperature;
                    }
                }

                // Додаємо метадані
                AddMetadata(temperatureGrid);

                // Зберігаємо результат
                bool saved = temperatureGrid.Save(outputPath, GridFileType.GeoTiff);

                bathymetryGrid.Close();
                temperatureGrid.Close();

                return saved;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при конвертації: {ex.Message}");
                return false;
            }
        }

        private double CalculateTemperature(double depth)
        {
            if (depth < 0) return -9999; // NoData value

            var depthLevels = new List<int>(_depthTemperatures.Keys);
            depthLevels.Sort();

            // Поверхневі води
            if (depth <= depthLevels[0])
            {
                var temp = _depthTemperatures[depthLevels[0]];
                return InterpolateTemperature(temp.minTemp, temp.maxTemp);
            }

            // Глибинні води
            if (depth >= depthLevels[depthLevels.Count - 1])
            {
                var temp = _depthTemperatures[depthLevels[depthLevels.Count - 1]];
                return InterpolateTemperature(temp.minTemp, temp.maxTemp);
            }

            // Знаходимо найближчі точки для інтерполяції
            int lowerDepth = 0;
            int upperDepth = 0;

            for (int i = 0; i < depthLevels.Count - 1; i++)
            {
                if (depth >= depthLevels[i] && depth < depthLevels[i + 1])
                {
                    lowerDepth = depthLevels[i];
                    upperDepth = depthLevels[i + 1];
                    break;
                }
            }

            // Інтерполяція між глибинами
            var lowerTemp = _depthTemperatures[lowerDepth];
            var upperTemp = _depthTemperatures[upperDepth];

            double ratio = (depth - lowerDepth) / (upperDepth - lowerDepth);
            double minTemp = Lerp(lowerTemp.minTemp, upperTemp.minTemp, ratio);
            double maxTemp = Lerp(lowerTemp.maxTemp, upperTemp.maxTemp, ratio);

            return InterpolateTemperature(minTemp, maxTemp);
        }

        private double Lerp(double start, double end, double ratio)
        {
            return start + (end - start) * ratio;
        }

        private double InterpolateTemperature(double minTemp, double maxTemp)
        {
            return minTemp + (_random.NextDouble() * (maxTemp - minTemp));
        }

        private void AddMetadata(Grid grid)
        {
            grid.Header.Notes = "Temperature Profile Generated from Bathymetry";
            grid.Header.NodataValue = -9999;
        }
    }

    public enum Season
    {
        Winter,
        Spring,
        Summer,
        Autumn
    }
}
