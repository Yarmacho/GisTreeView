using Entities;
using Entities.Entities;
using Interfaces.Database.Abstractions;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ExperimentTreeNode : EntityTreeNode<Experiment>
    {
        public int ExperimentId => Entity.Id;

        public ExperimentTreeNode(Experiment entity, IRepositoriesProvider repositoriesProvider)
            : base(entity, repositoriesProvider)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        protected override void OnUpdate(Experiment entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            menu.MenuItems.Add(new MenuItem("Add Gas", async (s, e) => await AppendChild<Gas, GasTreeNode>()));

            return menu;
        }
    }
}
