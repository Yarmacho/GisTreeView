using Entities.Entities;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class EnvironmentTreeNode : MapTreeNodeBase, INodeWithMap
    {
        private readonly ExperimentEnvironment _environment;

        public EnvironmentTreeNode(ExperimentEnvironment environment)
        {
            _environment = environment;
            Text = "Experiment environment";
        }

        Map INodeWithMap.Map => Map;

        public override string GetDescription()
        {
            return $"Sound reflection: {_environment.ReflectionCoef}";
        }

        protected override ContextMenu BuildContextMenu()
        {
            return new ContextMenu();
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
        }
    }
}
