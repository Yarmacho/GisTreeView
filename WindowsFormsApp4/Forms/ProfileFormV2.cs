using Entities.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
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
                    dataGridView1.Rows.Add(profil.Depth, profil.Temperature, profil.Salinity, profil.SoundSpeed, profil.Absorbsion);

                    temperatureSeries.Points.AddXY(profil.Depth, profil.Temperature);
                    salinitySeries.Points.AddXY(profil.Depth, profil.Salinity);
                    soundSpeedSeries.Points.AddXY(profil.Depth, profil.SoundSpeed);
                    absorbtionSeries.Points.AddXY(profil.Depth, profil.Absorbsion);
                }
            }

            configureChart(tempChart, temperatureSeries, battimetry, "Temperature", -20, 50);
            configureChart(salinityChart, salinitySeries, battimetry, "Salinity", 0, 100);
            configureChart(soundSpeedChart, soundSpeedSeries, battimetry, "Sound speed", 0, 100);
            configureChart(absorbtionChart, absorbtionSeries, battimetry, "Absorbtion", 0, 100);

            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void configureChart(Chart chart, Series series, MapWinGIS.Grid battimetry, string title, double min, double max)
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

            // Налаштування фону
            chart.ChartAreas[0].BackColor = Color.FromArgb(240, 240, 240);

            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = Math.Abs(Convert.ToDouble(battimetry.Minimum));

            var xInterval = chart.ChartAreas[0].AxisX.Maximum / 10;

            chart.ChartAreas[0].AxisY.Minimum = min;
            chart.ChartAreas[0].AxisY.Maximum = max;
            chart.ChartAreas[0].AxisX.Interval = xInterval;
            chart.ChartAreas[0].AxisY.Interval = 10;
            chart.ChartAreas[0].AxisX.Title = "Depth";
            chart.ChartAreas[0].AxisY.Title = title;

            chart.MouseClick += (s, e) =>
            {
                // Перетворення координат миші в координати графіка
                Point mousePoint = new Point(e.X, e.Y);
                ChartArea chartArea = chart.ChartAreas[0];

                double xValue = 0;
                double yValue = 0;

                try
                {
                    // Отримання координат кліку відносно графіка
                    xValue = roundToNearest((int)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X));
                    yValue = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                    // Перевірка, чи координати знаходяться в межах графіка
                    if (xValue >= chartArea.AxisX.Minimum && xValue <= chartArea.AxisX.Maximum &&
                        yValue >= chartArea.AxisY.Minimum && yValue <= chartArea.AxisY.Maximum)
                    {
                        var temperature = tabControl1.SelectedIndex == 0 ? yValue : 0;
                        var salinity = tabControl1.SelectedIndex == 1 ? yValue : 0;
                        var soundSpeed = tabControl1.SelectedIndex == 2 ? yValue : 0;
                        var absorbtion = tabControl1.SelectedIndex == 3 ? yValue : 0;

                        setValue(temperatureSeries, xValue, temperature);
                        setValue(salinitySeries, xValue, salinity);
                        setValue(soundSpeedSeries, xValue, soundSpeed);
                        setValue(absorbtionSeries, xValue, absorbtion);

                        sortAllSeriesPoints();

                        dataGridView1.Rows.Add(xValue, temperature, salinity, soundSpeed, absorbtion);
                        updateCharts();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при додаванні точки: {ex.Message}", "Помилка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var depth = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                temperatureSeries.Points[e.RowIndex].XValue = depth;
                salinitySeries.Points[e.RowIndex].XValue = depth;
                absorbtionSeries.Points[e.RowIndex].XValue = depth;
                soundSpeedSeries.Points[e.RowIndex].XValue = depth;
            }

            if (columnsSeries.TryGetValue(e.ColumnIndex, out var series))
            {
                var value = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                series.Points[e.RowIndex].SetValueY(value);
            }

            updateCharts();
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
            var point = series.Points.FirstOrDefault(p => (int)p.XValue == (int)depth);
            if (point != null)
            {
                point.SetValueY(value);
            }
            else
            {
                series.Points.Add(new DataPoint(depth, value));
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

        public List<Profil> Profiles
        {
            get
            {
                var profiles = new List<Profil>();
                for (var i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    var row = dataGridView1.Rows[i];
                    profiles.Add(new Profil()
                    {
                        Depth = Convert.ToDouble(row.Cells[0].Value),
                        Temperature = Convert.ToDouble(row.Cells[1].Value),
                        Salinity = Convert.ToDouble(row.Cells[2].Value),
                        SoundSpeed = Convert.ToDouble(row.Cells[3].Value),
                        Absorbsion = Convert.ToDouble(row.Cells[4].Value),
                    });
                }

                return profiles;
            }
        }
    }
}
