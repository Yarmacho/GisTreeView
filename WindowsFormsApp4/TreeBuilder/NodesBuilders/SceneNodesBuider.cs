using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class SceneNodesBuider : ShapeNodesBuilder<SceneTreeNode>
    {
        private readonly IReadOnlyDictionary<int, GasTreeNode> _gasNodes;
        public SceneNodesBuider(IReadOnlyDictionary<int, GasTreeNode> gasNodes)
        {
            _gasNodes = gasNodes;
        }

        public override async ValueTask<IEnumerable<SceneTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, SceneTreeNode>();

            var shapefile = buildNodesParams.Map.get_Shapefile(buildNodesParams.SceneLayerHandle);
            for (int i = 0; i < shapefile.NumShapes; i++)
            {
                var id = GetProperty<int>(shapefile, i, "SceneId");
                if (id == -1)
                {
                    continue;
                }

                var node = new SceneTreeNode(shapefile, i, buildNodesParams.SceneLayerHandle);
                nodes[id] = node;

                var gasId = GetProperty<int>(shapefile, i, "GasId");
                if (gasId != 0 && _gasNodes.TryGetValue(gasId, out var gasNode))
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
