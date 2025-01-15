namespace WindowsFormsApp4.Forms
{
    partial class RoutesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoutesForm));
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.submit = new System.Windows.Forms.Button();
            this.routePoints = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(132, 12);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(742, 426);
            this.axMap1.TabIndex = 10;
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(12, 415);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 20;
            this.submit.Text = "Create";
            this.submit.UseVisualStyleBackColor = true;
            // 
            // routePoints
            // 
            this.routePoints.Location = new System.Drawing.Point(13, 12);
            this.routePoints.Name = "routePoints";
            this.routePoints.Size = new System.Drawing.Size(113, 381);
            this.routePoints.TabIndex = 21;
            // 
            // RoutesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 450);
            this.Controls.Add(this.routePoints);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.axMap1);
            this.Name = "RoutesForm";
            this.Text = "TracePointForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.TreeView routePoints;
    }
}