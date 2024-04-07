using Entities.Entities;
using GeoDatabase.ORM;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class GasNodesBuilder : ShapeNodesBuilder<GasTreeNode, Gas>
    {
        private readonly IReadOnlyDictionary<int, ExperimentTreeNode> _experimentNodes;

        public GasNodesBuilder(IReadOnlyDictionary<int, ExperimentTreeNode> experimentNodes)
        {
            _experimentNodes = experimentNodes;
        }
        public GasNodesBuilder()
        {
        }

        public override async ValueTask<IEnumerable<GasTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, GasTreeNode>();
            var dbContext = buildNodesParams.ServiceProvider.GetRequiredService<GeoDbContext>();
            foreach (var gas in dbContext.Set<Gas>().ToList())
            {
                var shapeIndex = dbContext.ChangeTracker.GetShapeIndex(gas);
                var node = new GasTreeNode(gas, shapeIndex, buildNodesParams.GasLayerHandle);
                nodes[gas.Id] = node;

                if (_experimentNodes.TryGetValue(gas.ExperimentId, out var experimentTreeNode))
                {
                    experimentTreeNode.Nodes.Add(node);
                }
            }

            if (nodes.Count > 0 && buildNodesParams.SceneLayerHandle != -1)
            {
                await new SceneNodesBuider(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
