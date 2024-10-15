namespace WindowsFormsApp4.Forms
{
    partial class ShipForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShipForm));
            this.coordY = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.coordX = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.experimentId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.selectFromDict = new System.Windows.Forms.Button();
            this.submit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // coordY
            // 
            this.coordY.AutoSize = true;
            this.coordY.Location = new System.Drawing.Point(53, 204);
            this.coordY.Name = "coordY";
            this.coordY.Size = new System.Drawing.Size(35, 13);
            this.coordY.TabIndex = 17;
            this.coordY.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Y";
            // 
            // coordX
            // 
            this.coordX.AutoSize = true;
            this.coordX.Location = new System.Drawing.Point(53, 172);
            this.coordX.Name = "coordX";
            this.coordX.Size = new System.Drawing.Size(35, 13);
            this.coordX.TabIndex = 15;
            this.coordX.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "X";
            // 
            // experimentId
            // 
            this.experimentId.Location = new System.Drawing.Point(14, 137);
            this.experimentId.Name = "experimentId";
            this.experimentId.Size = new System.Drawing.Size(100, 20);
            this.experimentId.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(11, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Scene id";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(15, 77);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(100, 20);
            this.name.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Name";
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(159, 12);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(719, 426);
            this.axMap1.TabIndex = 9;
            // 
            // selectFromDict
            // 
            this.selectFromDict.Location = new System.Drawing.Point(12, 12);
            this.selectFromDict.Name = "selectFromDict";
            this.selectFromDict.Size = new System.Drawing.Size(127, 24);
            this.selectFromDict.TabIndex = 18;
            this.selectFromDict.Text = "Select from dictionary";
            this.selectFromDict.UseVisualStyleBackColor = true;
            this.selectFromDict.Click += new System.EventHandler(this.selectFromDict_Click);
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(64, 415);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 19;
            this.submit.Text = "Create";
            this.submit.UseVisualStyleBackColor = true;
            // 
            // ShipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 450);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.selectFromDict);
            this.Controls.Add(this.coordY);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.coordX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.experimentId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axMap1);
            this.Name = "ShipForm";
            this.Text = "ShipForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label coordY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label coordX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox experimentId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Button selectFromDict;
        private System.Windows.Forms.Button submit;
    }
}