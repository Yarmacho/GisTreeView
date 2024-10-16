﻿using Interfaces.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4
{
    internal class MapTreeView : TreeView
    {
        private Button button1;

        public Map Map { get; internal set; }

        public void Redraw()
        {
            Map = MapInitializer.Init(Map.AxMap);
            foreach (var node in Nodes.OfType<MapTreeNodeBase>())
            {
                node.SetMap(Map);
            }
        }


        public IRepositoriesProvider RepositoriesProvider => ServiceProvider.GetRequiredService<IRepositoriesProvider>();

        public IServiceProvider ServiceProvider { get; internal set; }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.ResumeLayout(false);
        }
    }
}
