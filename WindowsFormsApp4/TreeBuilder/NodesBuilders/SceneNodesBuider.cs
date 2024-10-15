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
        private readonly IReadOnlyDictionary<int, GasTreeNode> _gasNodes;
        public SceneNodesBuider(IReadOnlyDictionary<int, GasTreeNode> gasNodes)
        {
            _gasNodes = gasNodes;
        }

        public override async ValueTask<IEnumerable<SceneTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, SceneTreeNode>();

            var dbContext = buildNodesParams.ServiceProvider.GetRequiredService<GeoDbContext>();
            foreach (var scene in dbContext.Set<Scene>().ToList())
            {
                var node = new SceneTreeNode(scene);
                nodes[scene.Id] = node;

                if (_gasNodes.TryGetValue(scene.GasId, out var gasNode))
                {
                    gasNode.AddNode(node);
                }
            }

            if (nodes.Count > 0 && buildNodesParams.ShipLayerHandle != -1)
            {
                await new ShipNodesBuilder(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
