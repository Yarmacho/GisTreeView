namespace Forms.Forms
{
    partial class GasForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GasForm));
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.label1 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.experimentId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.coordX = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.coordY = new System.Windows.Forms.Label();
            this.depth = new System.Windows.Forms.Label();
            this.ZoomOut = new System.Windows.Forms.Button();
            this.ZoomIn = new System.Windows.Forms.Button();
            this.addShape = new System.Windows.Forms.Button();
            this.panBtn = new System.Windows.Forms.Button();
            this.selectFromDict = new System.Windows.Forms.Button();
            this.submit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(183, 48);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(712, 416);
            this.axMap1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(35, 97);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(100, 20);
            this.name.TabIndex = 2;
            // 
            // experimentId
            // 
            this.experimentId.Location = new System.Drawing.Point(34, 157);
            this.experimentId.Name = "experimentId";
            this.experimentId.Size = new System.Drawing.Size(100, 20);
            this.experimentId.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ExperimentId";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "X";
            // 
            // coordX
            // 
            this.coordX.AutoSize = true;
            this.coordX.Location = new System.Drawing.Point(73, 192);
            this.coordX.Name = "coordX";
            this.coordX.Size = new System.Drawing.Size(35, 13);
            this.coordX.TabIndex = 6;
            this.coordX.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 224);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Y";
            // 
            // coordY
            // 
            this.coordY.AutoSize = true;
            this.coordY.Location = new System.Drawing.Point(73, 224);
            this.coordY.Name = "coordY";
            this.coordY.Size = new System.Drawing.Size(35, 13);
            this.coordY.TabIndex = 8;
            this.coordY.Text = "Name";
            // 
            // depth
            // 
            this.depth.AutoSize = true;
            this.depth.Location = new System.Drawing.Point(807, 22);
            this.depth.Name = "depth";
            this.depth.Size = new System.Drawing.Size(42, 13);
            this.depth.TabIndex = 10;
            this.depth.Text = "Depth: ";
            // 
            // ZoomOut
            // 
            this.ZoomOut.Location = new System.Drawing.Point(424, 9);
            this.ZoomOut.Name = "ZoomOut";
            this.ZoomOut.Size = new System.Drawing.Size(75, 23);
            this.ZoomOut.TabIndex = 14;
            this.ZoomOut.Text = "Zoom Out";
            this.ZoomOut.UseVisualStyleBackColor = true;
            this.ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
            // 
            // ZoomIn
            // 
            this.ZoomIn.Location = new System.Drawing.Point(343, 9);
            this.ZoomIn.Name = "ZoomIn";
            this.ZoomIn.Size = new System.Drawing.Size(75, 23);
            this.ZoomIn.TabIndex = 13;
            this.ZoomIn.Text = "Zoom In";
            this.ZoomIn.UseVisualStyleBackColor = true;
            this.ZoomIn.Click += new System.EventHandler(this.ZoomIn_Click);
            // 
            // addShape
            // 
            this.addShape.Location = new System.Drawing.Point(262, 11);
            this.addShape.Name = "addShape";
            this.addShape.Size = new System.Drawing.Size(75, 21);
            this.addShape.TabIndex = 12;
            this.addShape.Text = "Add shape";
            this.addShape.UseVisualStyleBackColor = true;
            this.addShape.Click += new System.EventHandler(this.addShape_Click);
            // 
            // panBtn
            // 
            this.panBtn.Location = new System.Drawing.Point(181, 9);
            this.panBtn.Name = "panBtn";
            this.panBtn.Size = new System.Drawing.Size(75, 23);
            this.panBtn.TabIndex = 11;
            this.panBtn.Text = "Pan";
            this.panBtn.UseVisualStyleBackColor = true;
            this.panBtn.Click += new System.EventHandler(this.panBtn_Click);
            // 
            // selectFromDict
            // 
            this.selectFromDict.Location = new System.Drawing.Point(24, 11);
            this.selectFromDict.Name = "selectFromDict";
            this.selectFromDict.Size = new System.Drawing.Size(127, 24);
            this.selectFromDict.TabIndex = 15;
            this.selectFromDict.Text = "Select from dictionary";
            this.selectFromDict.UseVisualStyleBackColor = true;
            this.selectFromDict.Click += new System.EventHandler(this.selectFromDict_Click);
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(636, 9);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 16;
            this.submit.Text = "Create";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.submit_Click);
            // 
            // GasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 483);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.selectFromDict);
            this.Controls.Add(this.ZoomOut);
            this.Controls.Add(this.ZoomIn);
            this.Controls.Add(this.addShape);
            this.Controls.Add(this.panBtn);
            this.Controls.Add(this.depth);
            this.Controls.Add(this.coordY);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.coordX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.experimentId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axMap1);
            this.Name = "GasForm";
            this.Text = "GasForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.TextBox experimentId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label coordX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label coordY;
        private System.Windows.Forms.Label depth;
        private System.Windows.Forms.Button ZoomOut;
        private System.Windows.Forms.Button ZoomIn;
        private System.Windows.Forms.Button addShape;
        private System.Windows.Forms.Button panBtn;
        private System.Windows.Forms.Button selectFromDict;
        private System.Windows.Forms.Button submit;
    }
}