using Entities.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            MarkerSize = 3
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

            chart1.Series.Clear();
            chart1.Series.Add(temperatureSeries);
            chart1.Series.Add(salinitySeries);
            chart1.Series.Add(soundSpeedSeries);
            chart1.Series.Add(absorbtionSeries);
            chart1.BackColor = Color.WhiteSmoke;
            chart1.BorderlineColor = Color.LightGray;
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineWidth = 1;

            // Налаштування сітки
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY.MinorGrid.LineColor = Color.FromArgb(230, 230, 230);

            // Налаштування фону
            chart1.ChartAreas[0].BackColor = Color.FromArgb(240, 240, 240);

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = Math.Abs(Convert.ToDouble(battimetry.Minimum));

            var xInterval = chart1.ChartAreas[0].AxisX.Maximum / 10;

            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 100;
            chart1.ChartAreas[0].AxisX.Interval = xInterval;
            chart1.ChartAreas[0].AxisY.Interval = 10;
            chart1.ChartAreas[0].AxisX.Title = "Depth";
            chart1.ChartAreas[0].AxisY.Title = "Profiles value";

            chart1.MouseClick += chart_MouseClick;

            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
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

            chart1.Update();
            chart1.Refresh();
        }

        private void chart_MouseClick(object sender, MouseEventArgs e)
        {
            // Перетворення координат миші в координати графіка
            Point mousePoint = new Point(e.X, e.Y);
            ChartArea chartArea = chart1.ChartAreas[0];

            double xValue = 0;
            double yValue = 0;

            try
            {
                // Отримання координат кліку відносно графіка
                xValue = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                yValue = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                // Перевірка, чи координати знаходяться в межах графіка
                if (xValue >= chartArea.AxisX.Minimum && xValue <= chartArea.AxisX.Maximum &&
                    yValue >= chartArea.AxisY.Minimum && yValue <= chartArea.AxisY.Maximum)
                {
                    var form = new DepthProfileForm(xValue, yValue, yValue, yValue, yValue);
                    form.ShowDialog();

                    temperatureSeries.Points.Add(new DataPoint(xValue, form.Temperature)
                    {
                        ToolTip = $"X: {Math.Round(xValue, 2)}, Y: {Math.Round(yValue, 2)}"
                    });

                    salinitySeries.Points.Add(new DataPoint(xValue, form.Salinity)
                    {
                        ToolTip = $"X: {Math.Round(xValue, 2)}, Y: {Math.Round(yValue, 2)}"
                    });

                    soundSpeedSeries.Points.Add(new DataPoint(xValue, form.SoundSpeed)
                    {
                        ToolTip = $"X: {Math.Round(xValue, 2)}, Y: {Math.Round(yValue, 2)}"
                    });

                    absorbtionSeries.Points.Add(new DataPoint(xValue, form.Absorbtion)
                    {
                        ToolTip = $"X: {Math.Round(xValue, 2)}, Y: {Math.Round(yValue, 2)}"
                    });

                    dataGridView1.Rows.Add(xValue, form.Temperature, form.Salinity, form.SoundSpeed, form.Absorbtion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні точки: {ex.Message}", "Помилка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
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
