﻿
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.addExperimentBtn = new System.Windows.Forms.Button();
            this.treeView1 = new WindowsFormsApp4.MapTreeView();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(205, 26);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(583, 412);
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
            // mapTreeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(13, 55);
            this.treeView1.Name = "mapTreeView1";
            this.treeView1.Size = new System.Drawing.Size(186, 383);
            this.treeView1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.addExperimentBtn);
            this.Controls.Add(this.axMap1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Button addExperimentBtn;
        private MapTreeView treeView1;
    }
}

