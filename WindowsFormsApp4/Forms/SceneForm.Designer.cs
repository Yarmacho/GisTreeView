namespace Forms.Forms
{
    partial class SceneForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneForm));
            this.experimentId = new System.Windows.Forms.TextBox();
            this.gasId = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.angle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.side = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.submit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // experimentId
            // 
            this.experimentId.Enabled = false;
            this.experimentId.Location = new System.Drawing.Point(15, 109);
            this.experimentId.Name = "experimentId";
            this.experimentId.Size = new System.Drawing.Size(100, 20);
            this.experimentId.TabIndex = 13;
            // 
            // gasId
            // 
            this.gasId.AutoSize = true;
            this.gasId.Location = new System.Drawing.Point(12, 82);
            this.gasId.Name = "gasId";
            this.gasId.Size = new System.Drawing.Size(37, 13);
            this.gasId.TabIndex = 12;
            this.gasId.Text = "Gas id";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(16, 49);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(100, 20);
            this.name.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Name";
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(131, 12);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(657, 426);
            this.axMap1.TabIndex = 9;
            // 
            // angle
            // 
            this.angle.Location = new System.Drawing.Point(15, 167);
            this.angle.Name = "angle";
            this.angle.Size = new System.Drawing.Size(100, 20);
            this.angle.TabIndex = 15;
            this.angle.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Angle";
            // 
            // side
            // 
            this.side.Location = new System.Drawing.Point(16, 227);
            this.side.Name = "side";
            this.side.Size = new System.Drawing.Size(100, 20);
            this.side.TabIndex = 17;
            this.side.Text = "10";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Side";
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(16, 401);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 18;
            this.submit.Text = "Create";
            this.submit.UseVisualStyleBackColor = true;
            // 
            // SceneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.side);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.angle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.experimentId);
            this.Controls.Add(this.gasId);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axMap1);
            this.Name = "SceneForm";
            this.Text = "SceneForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox experimentId;
        private System.Windows.Forms.Label gasId;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.TextBox angle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox side;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button submit;
    }
}