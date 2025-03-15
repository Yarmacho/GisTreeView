using Entities.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp4.Forms
{
    public partial class ProfileFormV2 : Form 
    {
        private Series temperatureSeries = new Series("temperature")
        {
            ChartType = SeriesChartType.Line,
            Color = Color.Red,
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 3,
        };

        private Series salinitySeries = new Series("salinity")
        {
            ChartType = SeriesChartType.Line,
            Color = Color.LightBlue,
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 3
        };

        private Series soundSpeedSeries = new Series("sound speed")
        {
            ChartType = SeriesChartType.Line,
            Color = Color.YellowGreen,
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 3
        };

        private Series absorbtionSeries = new Series("absorbsion")
        {
            ChartType = SeriesChartType.Line,
            Color = Color.Lime,
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 3
        };

        private readonly IReadOnlyDictionary<int, Series> columnsSeries;

        public ProfileFormV2(MapWinGIS.Grid battimetry, IEnumerable<Profil> profiles = null)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = button1;
            AcceptButton.DialogResult = DialogResult.OK;

            columnsSeries = new Dictionary<int, Series>()
            {
                [1] = temperatureSeries,
                [2] = salinitySeries,
                [3] = soundSpeedSeries,
                [4] = absorbtionSeries,
            };

            if (profiles != null)
            {
                foreach (var profil in profiles)
                {
                    temperatureGrid.Rows.Add(profil.Depth, profil.Temperature);
                    salinityGrid.Rows.Add(profil.Depth, profil.Salinity);
                    speedGrid.Rows.Add(profil.Depth, profil.SoundSpeed);
                    absrobtionGrid.Rows.Add(profil.Depth, profil.Absorbsion);

                    temperatureSeries.Points.AddXY(profil.Temperature, profil.Depth);
                    salinitySeries.Points.AddXY(profil.Salinity, profil.Depth);
                    soundSpeedSeries.Points.AddXY(profil.SoundSpeed, profil.Depth);
                    absorbtionSeries.Points.AddXY(profil.Absorbsion, profil.Depth);
                }
                sortAllSeriesPoints();
            }

            configureChart(tempChart, temperatureGrid, temperatureSeries, battimetry, "Temperature", -20, 50);
            configureChart(salinityChart, salinityGrid, salinitySeries, battimetry, "Salinity", 0, 100);
            configureChart(soundSpeedChart, speedGrid, soundSpeedSeries, battimetry, "Sound speed", 0, 100);
            configureChart(absorbtionChart, absrobtionGrid, absorbtionSeries, battimetry, "Absorbtion", 0, 100);

            configureGridView(tempChart, temperatureGrid, temperatureSeries);
            configureGridView(salinityChart, salinityGrid, salinitySeries);
            configureGridView(soundSpeedChart, speedGrid, soundSpeedSeries);
            configureGridView(absorbtionChart, absrobtionGrid, absorbtionSeries);
        }

        private void configureChart(Chart chart, DataGridView gridView, Series series, MapWinGIS.Grid battimetry, string title, double min, double max)
        {
            chart.Series.Clear();
            chart.Series.Add(series);
            chart.BackColor = Color.WhiteSmoke;
            chart.BorderlineColor = Color.LightGray;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 1;

            // Налаштування сітки
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chart.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MinorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chart.ChartAreas[0].AxisY.IsReversed = true;

            // Налаштування фону
            chart.ChartAreas[0].BackColor = Color.FromArgb(240, 240, 240);

            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = Math.Abs(Convert.ToDouble(battimetry.Minimum));
            
            var yInterval = Math.Abs(Convert.ToDouble(battimetry.Minimum)) / 10;
            chart.ChartAreas[0].AxisY.Interval = yInterval;
            chart.ChartAreas[0].AxisY.Title = "Depth";

            chart.ChartAreas[0].AxisX.Minimum = min;
            chart.ChartAreas[0].AxisX.Maximum = max;
            chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisX.Title = title;

            chart.MouseClick += (s, e) =>
            {
                // Перетворення координат миші в координати графіка
                Point mousePoint = new Point(e.X, e.Y);
                ChartArea chartArea = chart.ChartAreas[0];

                double profileValue = 0;
                double depthValue = 0;

                try
                {
                    // Отримання координат кліку відносно графіка
                    depthValue = roundToNearest((int)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
                    profileValue = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);

                    // Перевірка, чи координати знаходяться в межах графіка
                    if (profileValue >= chartArea.AxisX.Minimum && profileValue <= chartArea.AxisX.Maximum &&
                        depthValue >= chartArea.AxisY.Minimum && depthValue <= chartArea.AxisY.Maximum)
                    {
                        profileValue = tabControl1.SelectedIndex == 0
                            ? Math.Round(profileValue, 1)
                            : Math.Round(profileValue, 3);

                        setValue(series, depthValue, profileValue);
                        sortSeriesPoints(series);

                        var row = gridView.Rows.OfType<DataGridViewRow>()
                            .FirstOrDefault(r => Convert.ToInt32(r.Cells[0].Value) == depthValue);
                        if (row == null)
                        {
                            gridView.Rows.Add(depthValue, profileValue);
                        }
                        else
                        {
                            row.Cells[1].Value = profileValue;
                        }

                        updateChart(chart);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при додаванні точки: {ex.Message}", "Помилка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }


        private void configureGridView(Chart chart, DataGridView gridView, Series series)
        {
            gridView.CellValueChanged += (s, e) =>
            {
                var depth = Convert.ToDouble(gridView.Rows[e.RowIndex].Cells[0].Value);
                var depthPoint = series.Points.FirstOrDefault(p => p.XValue == depth);
                if (depthPoint == null)
                {
                    return;
                }

                if (e.ColumnIndex == 0)
                {
                    depthPoint.XValue = depth;
                }
                else
                {
                    var value = Convert.ToDouble(gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    depthPoint.SetValueY(value);
                }

                updateChart(chart);
            };
        }

        private void updateCharts()
        {
            updateChart(tempChart);
            updateChart(salinityChart);
            updateChart(soundSpeedChart);
            updateChart(absorbtionChart);
        }

        private void updateChart(Chart chart)
        {
            chart.Update();
            chart.Refresh();
        }

        private void sortAllSeriesPoints()
        {
            sortSeriesPoints(temperatureSeries);
            sortSeriesPoints(salinitySeries);
            sortSeriesPoints(soundSpeedSeries);
            sortSeriesPoints(absorbtionSeries);
        }

        private void sortSeriesPoints(Series series)
        {
            var points = series.Points.OrderBy(p => p.XValue).ToList();

            series.Points.Clear();

            foreach (var point in points)
            {
                series.Points.Add(point);
            }
        }

        private void setValue(Series series, double depth, double value)
        {
            var point = series.Points.FirstOrDefault(p => p.YValues.Any(v => (int)v == (int)depth));
            if (point != null)
            {
                point.SetValueY(value);
            }
            else
            {
                series.Points.Add(new DataPoint(value, depth));
            }
        }

        private int roundToNearest(int value)
        {
            const int roundStep = 10;

            // Знаходимо остачу від ділення на 10
            int remainder = value % roundStep;

            // Якщо остача менше 5, округлюємо вниз, інакше - вгору
            if (remainder < 5)
            {
                return value - remainder;
            }
            else
            {
                return value + (roundStep - remainder);
            }
        }

        private static double calculateWilsonFormula(double temperature, double salinity, double depth)
        {
            // Формула Вільсона (спрощена версія)
            // C(T,S,P) = 1449.2 + 4.6T - 0.055T^2 + 0.00029T^3 + (1.34 - 0.01T)(S - 35) + 0.016D
            // де T - температура (°C), S - солоність (ppt), D - глибина (м)

            double t = temperature;
            double s = salinity;
            double d = depth;

            double c = 1449.2 + 4.6 * t - 0.055 * t * t + 0.00029 * t * t * t +
                      (1.34 - 0.01 * t) * (s - 35) + 0.016 * d;

            return Math.Round(c, 3);
        }

        private static double calculateThropFormula(double temperature, double salinity, double depth, double frequency = 10d)
        {
            // Спрощена формула Торпа для поглинання звуку
            // α = (0.11 * f^2 / (1 + f^2)) + (44 * f^2 / (4100 + f^2)) + (2.75 * 10^-4 * f^2) + 0.003
            // де f - частота в кГц

            double f = frequency; // частота в кГц
            double f2 = f * f;

            // Базове поглинання за формулою Торпа
            double absorption = (0.11 * f2 / (1 + f2)) +
                               (44 * f2 / (4100 + f2)) +
                               (2.75e-4 * f2) + 0.003;

            // Корекція для температури (спрощено)
            absorption *= Math.Pow(1.017, (temperature - 15.0));

            // Корекція для солоності (спрощено)
            if (salinity < 30)
            {
                absorption *= 0.95 + 0.0017 * salinity;
            }

            // Корекція для тиску/глибини (спрощено)
            double pressureCorrection = 1.0 - (0.00005 * depth);
            absorption *= pressureCorrection;

            return Math.Round(absorption, 3);
        }

        public List<Profil> Profiles
        {
            get
            {
                var tempProfiles = temperatureGrid.Rows.OfType<DataGridViewRow>()
                    .ToDictionary(r => Convert.ToInt32(r.Cells[0].Value), r => Convert.ToDouble(r.Cells[1].Value));
                var salinityProfiles = salinityGrid.Rows.OfType<DataGridViewRow>()
                    .ToDictionary(r => Convert.ToInt32(r.Cells[0].Value), r => Convert.ToDouble(r.Cells[1].Value));
                var soundProfiles = speedGrid.Rows.OfType<DataGridViewRow>()
                    .ToDictionary(r => Convert.ToInt32(r.Cells[0].Value), r => Convert.ToDouble(r.Cells[1].Value));
                var absorbtionProfiles = absrobtionGrid.Rows.OfType<DataGridViewRow>()
                    .ToDictionary(r => Convert.ToInt32(r.Cells[0].Value), r => Convert.ToDouble(r.Cells[1].Value));

                var depths = new HashSet<int>(tempProfiles.Keys);
                depths.UnionWith(salinityProfiles.Keys);
                depths.UnionWith(soundProfiles.Keys);
                depths.UnionWith(absorbtionProfiles.Keys);

                var profiles = new List<Profil>();
                foreach (var depth in depths)
                {
                    tempProfiles.TryGetValue(depth, out var temperature);
                    salinityProfiles.TryGetValue(depth, out var salinity);

                    if (!soundProfiles.TryGetValue(depth, out var soundSpeed))
                    {
                        soundSpeed = calculateWilsonFormula(temperature, salinity, depth);
                    }

                    if (!absorbtionProfiles.TryGetValue(depth, out var absorbtion))
                    {
                        absorbtion = calculateThropFormula(temperature, salinity, depth);
                    }

                    profiles.Add(new Profil()
                    {
                        Depth = depth,
                        Temperature = temperature,
                        SoundSpeed = soundSpeed,
                        Absorbsion = absorbtion,
                        Salinity = salinity,
                    });
                }

                return profiles;
            }
        }
    }
}
