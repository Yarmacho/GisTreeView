namespace WindowsFormsApp4.Forms
{
    partial class ProfilesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilesForm));
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.salinityBtn = new System.Windows.Forms.RadioButton();
            this.temperatureBtn = new System.Windows.Forms.RadioButton();
            this.noProfile = new System.Windows.Forms.RadioButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.depthSlider = new System.Windows.Forms.TrackBar();
            this.depthLabel = new System.Windows.Forms.Label();
            this.calcProfiles = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.depthSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(187, 55);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(601, 383);
            this.axMap1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.salinityBtn);
            this.groupBox1.Controls.Add(this.temperatureBtn);
            this.groupBox1.Controls.Add(this.noProfile);
            this.groupBox1.Location = new System.Drawing.Point(509, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 37);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // salinityBtn
            // 
            this.salinityBtn.AutoSize = true;
            this.salinityBtn.Location = new System.Drawing.Point(185, 14);
            this.salinityBtn.Name = "salinityBtn";
            this.salinityBtn.Size = new System.Drawing.Size(58, 17);
            this.salinityBtn.TabIndex = 2;
            this.salinityBtn.TabStop = true;
            this.salinityBtn.Text = "Salinity";
            this.salinityBtn.UseVisualStyleBackColor = true;
            // 
            // temperatureBtn
            // 
            this.temperatureBtn.AutoSize = true;
            this.temperatureBtn.Location = new System.Drawing.Point(94, 14);
            this.temperatureBtn.Name = "temperatureBtn";
            this.temperatureBtn.Size = new System.Drawing.Size(85, 17);
            this.temperatureBtn.TabIndex = 1;
            this.temperatureBtn.TabStop = true;
            this.temperatureBtn.Text = "Temperature";
            this.temperatureBtn.UseVisualStyleBackColor = true;
            // 
            // noProfile
            // 
            this.noProfile.AutoSize = true;
            this.noProfile.Location = new System.Drawing.Point(6, 14);
            this.noProfile.Name = "noProfile";
            this.noProfile.Size = new System.Drawing.Size(70, 17);
            this.noProfile.TabIndex = 0;
            this.noProfile.TabStop = true;
            this.noProfile.Text = "No profile";
            this.noProfile.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 55);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(131, 383);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // depthSlider
            // 
            this.depthSlider.BackColor = System.Drawing.SystemColors.Control;
            this.depthSlider.Location = new System.Drawing.Point(136, 55);
            this.depthSlider.Name = "depthSlider";
            this.depthSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.depthSlider.Size = new System.Drawing.Size(45, 383);
            this.depthSlider.TabIndex = 4;
            // 
            // depthLabel
            // 
            this.depthLabel.AutoSize = true;
            this.depthLabel.Location = new System.Drawing.Point(136, 26);
            this.depthLabel.Name = "depthLabel";
            this.depthLabel.Size = new System.Drawing.Size(36, 13);
            this.depthLabel.TabIndex = 5;
            this.depthLabel.Text = "Depth";
            // 
            // calcProfiles
            // 
            this.calcProfiles.Location = new System.Drawing.Point(375, 26);
            this.calcProfiles.Name = "calcProfiles";
            this.calcProfiles.Size = new System.Drawing.Size(113, 23);
            this.calcProfiles.TabIndex = 6;
            this.calcProfiles.Text = "Calculate profiles";
            this.calcProfiles.UseVisualStyleBackColor = true;
            this.calcProfiles.Click += new System.EventHandler(this.calcProfiles_Click);
            // 
            // ProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.calcProfiles);
            this.Controls.Add(this.depthLabel);
            this.Controls.Add(this.depthSlider);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.axMap1);
            this.Name = "ProfilesForm";
            this.Text = "ProfilesForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.depthSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton salinityBtn;
        private System.Windows.Forms.RadioButton temperatureBtn;
        private System.Windows.Forms.RadioButton noProfile;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TrackBar depthSlider;
        private System.Windows.Forms.Label depthLabel;
        private System.Windows.Forms.Button calcProfiles;
    }
}