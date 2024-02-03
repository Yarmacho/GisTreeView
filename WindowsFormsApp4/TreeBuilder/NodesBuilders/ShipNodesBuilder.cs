using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class ShipNodesBuilder : ShapeNodesBuilder<ShipTreeNode>
    {
        private readonly IReadOnlyDictionary<int, SceneTreeNode> _sceneNodes;

        public ShipNodesBuilder(IReadOnlyDictionary<int, SceneTreeNode> sceneNodes)
        {
            _sceneNodes = sceneNodes;
        }

        public override async ValueTask<IEnumerable<ShipTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, ShipTreeNode>();

            var shapefile = buildNodesParams.Map.get_Shapefile(buildNodesParams.ShipLayerHandle);
            for (int i = 0; i < shapefile.NumShapes; i++)
            {
                var id = GetProperty<int>(shapefile, i, "ShipId");
                if (id == 0)
                {
                    continue;
                }

                var node = new ShipTreeNode(shapefile, i, buildNodesParams.ShipLayerHandle);
                nodes[id] = node;

                var sceneId = GetProperty<int>(shapefile, i, "SceneId");
                if (sceneId != 0 && _sceneNodes.TryGetValue(sceneId, out var sceneNode))
                {
                    sceneNode.AddNode(node);
                }
            }

            if (nodes.Count > 0 && buildNodesParams.ProfileLayerHandle != -1)
            {
                new ProfilNodesBuilder(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
