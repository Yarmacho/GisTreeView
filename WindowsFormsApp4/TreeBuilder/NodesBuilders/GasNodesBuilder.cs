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
        private readonly IReadOnlyDictionary<int, SceneTreeNode> _sceneNodes;

        public GasNodesBuilder(IReadOnlyDictionary<int, SceneTreeNode> sceneNodes)
        {
            _sceneNodes = sceneNodes;
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
                var node = new GasTreeNode(gas);
                nodes[gas.Id] = node;

                if (_sceneNodes.TryGetValue(gas.SceneId, out var sceneNode))
                {
                    sceneNode.Nodes.Add(node);
                }
            }

            foreach (var node in nodes.Values)
            {
                node.SetMap(buildNodesParams.Map);
            }

            return nodes.Values;
        }
    }
}
