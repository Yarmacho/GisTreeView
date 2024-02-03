using Entities;
using Interfaces.Database.Abstractions;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ExperimentTreeNode : EntityTreeNode<Experiment, int>
    {
        public int ExperimentId => Entity.Id;

        public ExperimentTreeNode(Experiment entity, IRepositoriesProvider repositoriesProvider)
            : base(entity, repositoriesProvider)
        {
            Name = entity.Name;
            Text = entity.Name;
        }
    }
}
