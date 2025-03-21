﻿namespace WindowsFormsApp4.Forms
{
    partial class DepthProfileForm
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
            this.depth = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.temperature = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.salinity = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.speed = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.absorbtion = new WindowsFormsApp4.Components.NumericTextBox(this.components);
            this.SuspendLayout();
            // 
            // depth
            // 
            this.depth.Location = new System.Drawing.Point(93, 24);
            this.depth.Name = "depth";
            this.depth.Size = new System.Drawing.Size(100, 20);
            this.depth.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Depth";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Temperature";
            // 
            // temperature
            // 
            this.temperature.Location = new System.Drawing.Point(93, 63);
            this.temperature.Name = "temperature";
            this.temperature.Size = new System.Drawing.Size(100, 20);
            this.temperature.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Salinity";
            // 
            // salinity
            // 
            this.salinity.Location = new System.Drawing.Point(93, 89);
            this.salinity.Name = "salinity";
            this.salinity.Size = new System.Drawing.Size(100, 20);
            this.salinity.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Sound speed";
            // 
            // speed
            // 
            this.speed.Location = new System.Drawing.Point(93, 124);
            this.speed.Name = "speed";
            this.speed.Size = new System.Drawing.Size(100, 20);
            this.speed.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(117, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Confirm";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Absorbtion";
            // 
            // absorbtion
            // 
            this.absorbtion.Location = new System.Drawing.Point(92, 150);
            this.absorbtion.Name = "absorbtion";
            this.absorbtion.Size = new System.Drawing.Size(100, 20);
            this.absorbtion.TabIndex = 9;
            // 
            // DepthProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 206);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.absorbtion);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.speed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.salinity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.temperature);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.depth);
            this.Name = "DepthProfileForm";
            this.Text = "DepthProfileForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.NumericTextBox depth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Components.NumericTextBox temperature;
        private System.Windows.Forms.Label label3;
        private Components.NumericTextBox salinity;
        private System.Windows.Forms.Label label4;
        private Components.NumericTextBox speed;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private Components.NumericTextBox absorbtion;
    }
}