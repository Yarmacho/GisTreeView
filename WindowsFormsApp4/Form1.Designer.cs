
namespace WindowsFormsApp4
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.addExperimentBtn = new System.Windows.Forms.Button();
            this.entityDesc = new System.Windows.Forms.RichTextBox();
            this.depth = new System.Windows.Forms.Label();
            this.refresh = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ObjectsTree = new System.Windows.Forms.TabPage();
            this.layersManager = new System.Windows.Forms.TabPage();
            this.eventsDispathTimer = new System.Windows.Forms.Timer(this.components);
            this.axMap2 = new AxMapWinGIS.AxMap();
            this.axMap3 = new AxMapWinGIS.AxMap();
            this.temperatureLabel = new System.Windows.Forms.Label();
            this.salnityLabel = new System.Windows.Forms.Label();
            this.treeView1 = new WindowsFormsApp4.MapTreeView();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.ObjectsTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMap2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMap3)).BeginInit();
            this.SuspendLayout();
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(205, 55);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(583, 383);
            this.axMap1.TabIndex = 0;
            // 
            // addExperimentBtn
            // 
            this.addExperimentBtn.Location = new System.Drawing.Point(13, 26);
            this.addExperimentBtn.Name = "addExperimentBtn";
            this.addExperimentBtn.Size = new System.Drawing.Size(88, 23);
            this.addExperimentBtn.TabIndex = 2;
            this.addExperimentBtn.Text = "Add experiment";
            this.addExperimentBtn.UseVisualStyleBackColor = true;
            this.addExperimentBtn.Click += new System.EventHandler(this.addExperimentBtn_Click);
            // 
            // entityDesc
            // 
            this.entityDesc.Enabled = false;
            this.entityDesc.Location = new System.Drawing.Point(3, 444);
            this.entityDesc.Name = "entityDesc";
            this.entityDesc.Size = new System.Drawing.Size(785, 127);
            this.entityDesc.TabIndex = 4;
            this.entityDesc.Text = "";
            // 
            // depth
            // 
            this.depth.AutoSize = true;
            this.depth.Location = new System.Drawing.Point(696, 26);
            this.depth.Name = "depth";
            this.depth.Size = new System.Drawing.Size(39, 13);
            this.depth.TabIndex = 5;
            this.depth.Text = "Depth:";
            // 
            // refresh
            // 
            this.refresh.Location = new System.Drawing.Point(107, 26);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(88, 23);
            this.refresh.TabIndex = 6;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ObjectsTree);
            this.tabControl1.Controls.Add(this.layersManager);
            this.tabControl1.Location = new System.Drawing.Point(-1, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 383);
            this.tabControl1.TabIndex = 7;
            // 
            // ObjectsTree
            // 
            this.ObjectsTree.Controls.Add(this.treeView1);
            this.ObjectsTree.Location = new System.Drawing.Point(4, 22);
            this.ObjectsTree.Name = "ObjectsTree";
            this.ObjectsTree.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectsTree.Size = new System.Drawing.Size(192, 357);
            this.ObjectsTree.TabIndex = 0;
            this.ObjectsTree.Text = "Objects tree";
            this.ObjectsTree.UseVisualStyleBackColor = true;
            // 
            // layersManager
            // 
            this.layersManager.Location = new System.Drawing.Point(4, 22);
            this.layersManager.Name = "layersManager";
            this.layersManager.Padding = new System.Windows.Forms.Padding(3);
            this.layersManager.Size = new System.Drawing.Size(192, 357);
            this.layersManager.TabIndex = 1;
            this.layersManager.Text = "Layers manager";
            this.layersManager.UseVisualStyleBackColor = true;
            // 
            // eventsDispathTimer
            // 
            this.eventsDispathTimer.Enabled = true;
            this.eventsDispathTimer.Interval = 5000;
            // 
            // axMap2
            // 
            this.axMap2.Enabled = true;
            this.axMap2.Location = new System.Drawing.Point(806, 77);
            this.axMap2.Name = "axMap2";
            this.axMap2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap2.OcxState")));
            this.axMap2.Size = new System.Drawing.Size(393, 166);
            this.axMap2.TabIndex = 8;
            // 
            // axMap3
            // 
            this.axMap3.Enabled = true;
            this.axMap3.Location = new System.Drawing.Point(806, 272);
            this.axMap3.Name = "axMap3";
            this.axMap3.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap3.OcxState")));
            this.axMap3.Size = new System.Drawing.Size(393, 166);
            this.axMap3.TabIndex = 9;
            // 
            // temperatureLabel
            // 
            this.temperatureLabel.AutoSize = true;
            this.temperatureLabel.Location = new System.Drawing.Point(803, 55);
            this.temperatureLabel.Name = "temperatureLabel";
            this.temperatureLabel.Size = new System.Drawing.Size(70, 13);
            this.temperatureLabel.TabIndex = 10;
            this.temperatureLabel.Text = "Tempareture:";
            // 
            // salnityLabel
            // 
            this.salnityLabel.AutoSize = true;
            this.salnityLabel.Location = new System.Drawing.Point(803, 256);
            this.salnityLabel.Name = "salnityLabel";
            this.salnityLabel.Size = new System.Drawing.Size(43, 13);
            this.salnityLabel.TabIndex = 11;
            this.salnityLabel.Text = "Salinity:";
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(189, 357);
            this.treeView1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1211, 583);
            this.Controls.Add(this.salnityLabel);
            this.Controls.Add(this.temperatureLabel);
            this.Controls.Add(this.axMap3);
            this.Controls.Add(this.axMap2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.depth);
            this.Controls.Add(this.entityDesc);
            this.Controls.Add(this.addExperimentBtn);
            this.Controls.Add(this.axMap1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ObjectsTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axMap2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMap3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Button addExperimentBtn;
        private MapTreeView treeView1;
        private System.Windows.Forms.RichTextBox entityDesc;
        private System.Windows.Forms.Label depth;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ObjectsTree;
        private System.Windows.Forms.TabPage layersManager;
        private System.Windows.Forms.Timer eventsDispathTimer;
        private AxMapWinGIS.AxMap axMap2;
        private AxMapWinGIS.AxMap axMap3;
        private System.Windows.Forms.Label temperatureLabel;
        private System.Windows.Forms.Label salnityLabel;
    }
}

