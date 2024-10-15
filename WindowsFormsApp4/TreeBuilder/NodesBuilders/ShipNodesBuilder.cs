using Entities.Entities;
using GeoDatabase.ORM;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class ShipNodesBuilder : ShapeNodesBuilder<ShipTreeNode, Ship>
    {
        private readonly IReadOnlyDictionary<int, SceneTreeNode> _sceneNodes;

        public ShipNodesBuilder(IReadOnlyDictionary<int, SceneTreeNode> sceneNodes)
        {
            _sceneNodes = sceneNodes;
        }

        public override async ValueTask<IEnumerable<ShipTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, ShipTreeNode>();

            var dbContext = buildNodesParams.ServiceProvider.GetRequiredService<GeoDbContext>();
            foreach (var ship in dbContext.Set<Ship>().ToList())
            {
                var node = new ShipTreeNode(ship);
                nodes[ship.Id] = node;

                if (_sceneNodes.TryGetValue(ship.SceneId, out var sceneNode))
                {
                    sceneNode.AddNode(node);
                }
            }

            if (nodes.Count > 0)
            {
                if (buildNodesParams.RoutesLayerHandle != -1)
                {
                    await new RouteNodesBuilder(nodes).BuildNodes(buildNodesParams);
                }
            }

            return nodes.Values;
        }
    }
}
