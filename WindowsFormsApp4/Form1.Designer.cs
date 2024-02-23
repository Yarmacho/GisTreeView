
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
            this.entityDesc = new System.Windows.Forms.RichTextBox();
            this.depth = new System.Windows.Forms.Label();
            this.refresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
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
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(13, 55);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(186, 383);
            this.treeView1.TabIndex = 3;
            // 
            // entityDesc
            // 
            this.entityDesc.Enabled = false;
            this.entityDesc.Location = new System.Drawing.Point(806, 55);
            this.entityDesc.Name = "entityDesc";
            this.entityDesc.Size = new System.Drawing.Size(267, 383);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1085, 450);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.depth);
            this.Controls.Add(this.entityDesc);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.addExperimentBtn);
            this.Controls.Add(this.axMap1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
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
    }
}

