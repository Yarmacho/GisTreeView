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
            this.components = new System.ComponentModel.Container();
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
            this.label4 = new System.Windows.Forms.Label();
            this.AccelerationLabel = new System.Windows.Forms.Label();
            this.labelTurnRate = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.maxSpeed = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.turnRate = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.acceleration = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.deceleration = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.width = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.length = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.addShape = new System.Windows.Forms.Button();
            this.panBtn = new System.Windows.Forms.Button();
            this.depth = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // coordY
            // 
            this.coordY.AutoSize = true;
            this.coordY.Location = new System.Drawing.Point(61, 327);
            this.coordY.Name = "coordY";
            this.coordY.Size = new System.Drawing.Size(35, 13);
            this.coordY.TabIndex = 17;
            this.coordY.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 327);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Y";
            // 
            // coordX
            // 
            this.coordX.AutoSize = true;
            this.coordX.Location = new System.Drawing.Point(61, 295);
            this.coordX.Name = "coordX";
            this.coordX.Size = new System.Drawing.Size(35, 13);
            this.coordX.TabIndex = 15;
            this.coordX.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 295);
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
            this.axMap1.Location = new System.Drawing.Point(262, 50);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(719, 388);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Length";
            // 
            // AccelerationLabel
            // 
            this.AccelerationLabel.AutoSize = true;
            this.AccelerationLabel.Location = new System.Drawing.Point(142, 172);
            this.AccelerationLabel.Name = "AccelerationLabel";
            this.AccelerationLabel.Size = new System.Drawing.Size(66, 13);
            this.AccelerationLabel.TabIndex = 26;
            this.AccelerationLabel.Text = "Acceleration";
            // 
            // labelTurnRate
            // 
            this.labelTurnRate.AutoSize = true;
            this.labelTurnRate.Location = new System.Drawing.Point(141, 110);
            this.labelTurnRate.Name = "labelTurnRate";
            this.labelTurnRate.Size = new System.Drawing.Size(52, 13);
            this.labelTurnRate.TabIndex = 24;
            this.labelTurnRate.Text = "TurnRate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(142, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Max speed";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(139, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Deceleration";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 235);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Width";
            // 
            // maxSpeed
            // 
            this.maxSpeed.Location = new System.Drawing.Point(142, 77);
            this.maxSpeed.Name = "maxSpeed";
            this.maxSpeed.Size = new System.Drawing.Size(100, 20);
            this.maxSpeed.TabIndex = 37;
            // 
            // turnRate
            // 
            this.turnRate.Location = new System.Drawing.Point(142, 137);
            this.turnRate.Name = "turnRate";
            this.turnRate.Size = new System.Drawing.Size(100, 20);
            this.turnRate.TabIndex = 36;
            // 
            // acceleration
            // 
            this.acceleration.Location = new System.Drawing.Point(144, 188);
            this.acceleration.Name = "acceleration";
            this.acceleration.Size = new System.Drawing.Size(100, 20);
            this.acceleration.TabIndex = 35;
            // 
            // deceleration
            // 
            this.deceleration.Location = new System.Drawing.Point(142, 251);
            this.deceleration.Name = "deceleration";
            this.deceleration.Size = new System.Drawing.Size(100, 20);
            this.deceleration.TabIndex = 34;
            // 
            // width
            // 
            this.width.Location = new System.Drawing.Point(12, 251);
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(100, 20);
            this.width.TabIndex = 33;
            // 
            // length
            // 
            this.length.Location = new System.Drawing.Point(12, 188);
            this.length.Name = "length";
            this.length.Size = new System.Drawing.Size(100, 20);
            this.length.TabIndex = 32;
            // 
            // addShape
            // 
            this.addShape.Location = new System.Drawing.Point(343, 15);
            this.addShape.Name = "addShape";
            this.addShape.Size = new System.Drawing.Size(75, 21);
            this.addShape.TabIndex = 39;
            this.addShape.Text = "Add shape";
            this.addShape.UseVisualStyleBackColor = true;
            // 
            // panBtn
            // 
            this.panBtn.Location = new System.Drawing.Point(262, 13);
            this.panBtn.Name = "panBtn";
            this.panBtn.Size = new System.Drawing.Size(75, 23);
            this.panBtn.TabIndex = 38;
            this.panBtn.Text = "Pan";
            this.panBtn.UseVisualStyleBackColor = true;
            // 
            // depth
            // 
            this.depth.AutoSize = true;
            this.depth.Location = new System.Drawing.Point(857, 23);
            this.depth.Name = "depth";
            this.depth.Size = new System.Drawing.Size(39, 13);
            this.depth.TabIndex = 40;
            this.depth.Text = "Depth:";
            // 
            // ShipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 454);
            this.Controls.Add(this.depth);
            this.Controls.Add(this.addShape);
            this.Controls.Add(this.panBtn);
            this.Controls.Add(this.maxSpeed);
            this.Controls.Add(this.turnRate);
            this.Controls.Add(this.acceleration);
            this.Controls.Add(this.deceleration);
            this.Controls.Add(this.width);
            this.Controls.Add(this.length);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.AccelerationLabel);
            this.Controls.Add(this.labelTurnRate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelTurnRate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label AccelerationLabel;
        private Components.NumericTextBox length;
        private Components.NumericTextBox width;
        private Components.NumericTextBox deceleration;
        private Components.NumericTextBox acceleration;
        private Components.NumericTextBox turnRate;
        private Components.NumericTextBox maxSpeed;
        private System.Windows.Forms.Button addShape;
        private System.Windows.Forms.Button panBtn;
        private System.Windows.Forms.Label depth;
    }
}