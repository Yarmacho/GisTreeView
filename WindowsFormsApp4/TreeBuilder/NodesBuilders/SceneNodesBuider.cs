using Entities.Entities;
using GeoDatabase.ORM;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class SceneNodesBuider : ShapeNodesBuilder<SceneTreeNode, Scene>
    {
        private readonly IReadOnlyDictionary<int, ExperimentTreeNode> _experimentNodes;
        public SceneNodesBuider(IReadOnlyDictionary<int, ExperimentTreeNode> exprimentNodes)
        {
            _experimentNodes = exprimentNodes;
        }

        public override async ValueTask<IEnumerable<SceneTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, SceneTreeNode>();

            var dbContext = buildNodesParams.ServiceProvider.GetRequiredService<GeoDbContext>();
            foreach (var scene in dbContext.Set<Scene>().ToList())
            {
                var node = new SceneTreeNode(scene);
                nodes[scene.Id] = node;

                if (_experimentNodes.TryGetValue(scene.ExperimentId, out var experimentTreeNode))
                {
                    experimentTreeNode.Nodes.Add(node);
                }
            }

            if (nodes.Count > 0 && buildNodesParams.ShipLayerHandle != -1)
            {
                await new ShipNodesBuilder(nodes).BuildNodes(buildNodesParams);
            }

            if (nodes.Count > 0 && buildNodesParams.GasLayerHandle != -1)
            {
                await new GasNodesBuilder(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
