namespace WindowsFormsApp4.Forms
{
    partial class ProfileFormV2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.temperatureGrid = new System.Windows.Forms.DataGridView();
            this.Depth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Temperature = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tempChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.salinityChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.soundSpeedChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.absorbtionChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.salinityGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.speedGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.absrobtionGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureGrid)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tempChart)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.salinityChart)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundSpeedChart)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.absorbtionChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salinityGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.absrobtionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // temperatureGrid
            // 
            this.temperatureGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.temperatureGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Depth,
            this.Temperature});
            this.temperatureGrid.Location = new System.Drawing.Point(3, 4);
            this.temperatureGrid.Name = "temperatureGrid";
            this.temperatureGrid.Size = new System.Drawing.Size(270, 381);
            this.temperatureGrid.TabIndex = 1;
            // 
            // Depth
            // 
            this.Depth.HeaderText = "Depth";
            this.Depth.Name = "Depth";
            // 
            // Temperature
            // 
            this.Temperature.HeaderText = "Temperature";
            this.Temperature.Name = "Temperature";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1006, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Confirm";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 21);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1113, 417);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tempChart);
            this.tabPage1.Controls.Add(this.temperatureGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1105, 391);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Temperature";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tempChart
            // 
            this.tempChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.Center;
            chartArea5.Name = "ChartArea1";
            this.tempChart.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.tempChart.Legends.Add(legend5);
            this.tempChart.Location = new System.Drawing.Point(279, 6);
            this.tempChart.Name = "tempChart";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.tempChart.Series.Add(series5);
            this.tempChart.Size = new System.Drawing.Size(820, 379);
            this.tempChart.TabIndex = 1;
            this.tempChart.Text = "chart1";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.salinityGrid);
            this.tabPage2.Controls.Add(this.salinityChart);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1105, 391);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Salinity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // salinityChart
            // 
            this.salinityChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.Center;
            chartArea6.Name = "ChartArea1";
            this.salinityChart.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.salinityChart.Legends.Add(legend6);
            this.salinityChart.Location = new System.Drawing.Point(282, 0);
            this.salinityChart.Name = "salinityChart";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.salinityChart.Series.Add(series6);
            this.salinityChart.Size = new System.Drawing.Size(817, 385);
            this.salinityChart.TabIndex = 2;
            this.salinityChart.Text = "chart2";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.speedGrid);
            this.tabPage3.Controls.Add(this.soundSpeedChart);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1105, 391);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Sound speed";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // soundSpeedChart
            // 
            this.soundSpeedChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.Center;
            chartArea7.Name = "ChartArea1";
            this.soundSpeedChart.ChartAreas.Add(chartArea7);
            legend7.Name = "Legend1";
            this.soundSpeedChart.Legends.Add(legend7);
            this.soundSpeedChart.Location = new System.Drawing.Point(279, 6);
            this.soundSpeedChart.Name = "soundSpeedChart";
            series7.ChartArea = "ChartArea1";
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            this.soundSpeedChart.Series.Add(series7);
            this.soundSpeedChart.Size = new System.Drawing.Size(820, 379);
            this.soundSpeedChart.TabIndex = 2;
            this.soundSpeedChart.Text = "soundSpeedChart";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.absrobtionGrid);
            this.tabPage4.Controls.Add(this.absorbtionChart);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1105, 391);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Absorbtion";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // absorbtionChart
            // 
            this.absorbtionChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.Center;
            chartArea8.Name = "ChartArea1";
            this.absorbtionChart.ChartAreas.Add(chartArea8);
            legend8.Name = "Legend1";
            this.absorbtionChart.Legends.Add(legend8);
            this.absorbtionChart.Location = new System.Drawing.Point(279, 6);
            this.absorbtionChart.Name = "absorbtionChart";
            series8.ChartArea = "ChartArea1";
            series8.Legend = "Legend1";
            series8.Name = "Series1";
            this.absorbtionChart.Series.Add(series8);
            this.absorbtionChart.Size = new System.Drawing.Size(820, 379);
            this.absorbtionChart.TabIndex = 2;
            this.absorbtionChart.Text = "chart2";
            // 
            // salinityGrid
            // 
            this.salinityGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.salinityGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.salinityGrid.Location = new System.Drawing.Point(6, 4);
            this.salinityGrid.Name = "salinityGrid";
            this.salinityGrid.Size = new System.Drawing.Size(270, 381);
            this.salinityGrid.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Depth";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Salinity";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // speedGrid
            // 
            this.speedGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.speedGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.speedGrid.Location = new System.Drawing.Point(3, 6);
            this.speedGrid.Name = "speedGrid";
            this.speedGrid.Size = new System.Drawing.Size(270, 381);
            this.speedGrid.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Depth";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Sound speed";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // absrobtionGrid
            // 
            this.absrobtionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.absrobtionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.absrobtionGrid.Location = new System.Drawing.Point(3, 4);
            this.absrobtionGrid.Name = "absrobtionGrid";
            this.absrobtionGrid.Size = new System.Drawing.Size(270, 381);
            this.absrobtionGrid.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Depth";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Absorbtion";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // ProfileFormV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 476);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button1);
            this.Name = "ProfileFormV2";
            this.Text = "ProfileFormV2";
            ((System.ComponentModel.ISupportInitialize)(this.temperatureGrid)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tempChart)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.salinityChart)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.soundSpeedChart)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.absorbtionChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salinityGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.absrobtionGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView temperatureGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataVisualization.Charting.Chart tempChart;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataVisualization.Charting.Chart salinityChart;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataVisualization.Charting.Chart soundSpeedChart;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataVisualization.Charting.Chart absorbtionChart;
        private System.Windows.Forms.DataGridViewTextBoxColumn Depth;
        private System.Windows.Forms.DataGridViewTextBoxColumn Temperature;
        private System.Windows.Forms.DataGridView salinityGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView speedGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridView absrobtionGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}