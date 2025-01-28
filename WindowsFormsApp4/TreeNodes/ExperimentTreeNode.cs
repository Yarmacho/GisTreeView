﻿using Entities;
using Entities.Entities;
using Interfaces.Database.Abstractions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ExperimentTreeNode : EntityTreeNode<Experiment>
    {
        public int ExperimentId => Entity.Id;

        public ExperimentTreeNode(Experiment entity) : base(entity)
        {
            Name = entity.Name;
            Text = entity.Name;
            ImageKey = "experiment";
            SelectedImageKey = "experiment";
        }

        protected override void OnUpdate(Experiment entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            menu.MenuItems.Add(0, new MenuItem("Add scene", async (s, e) => await AppendChild<Scene, SceneTreeNode>()));

            return menu;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Gas gas)
            {
                gas.ExperimentId = ExperimentId;
            }
            else if (childEntity is Scene scene)
            {
                scene.ExperimentId = ExperimentId;
            }
        }
    }
}
