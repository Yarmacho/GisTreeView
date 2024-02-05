using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class GasNodesBuilder : ShapeNodesBuilder<GasTreeNode>
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
            var shapefile = buildNodesParams.Map.get_Shapefile(buildNodesParams.GasLayerHandle);
            for (int i = 0; i < shapefile.NumShapes; i++)
            {
                var id = GetProperty<int>(shapefile, i, "Id");
                if (id == 0)
                {
                    continue;
                }

                var node = new GasTreeNode(shapefile, i, buildNodesParams.GasLayerHandle);
                nodes[id] = node;
            }

            if (nodes.Count > 0 && buildNodesParams.SceneLayerHandle != -1)
            {
                await new SceneNodesBuider(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
